using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using System;
using Terraria.Audio;

namespace Armorillose.Content.Items.Weapons
{
    public class PrismaticGlaive : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prismatic Glaive"); // Not needed in 1.4+
            /* Tooltip.SetDefault("Shoots a prismatic projectile that bounces off surfaces" +
                "\nEach bounce changes the projectile's color and increases damage"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(gold: 3, silver: 50);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PrismaticGlaiveProjectile>();
            Item.shootSpeed = 12f;
            Item.noMelee = false; // The projectile will do the damage and not the weapon itself
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddIngredient(ItemID.CrystalShard, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;

            return true;
        }
    }

    public class PrismaticGlaiveProjectile : ModProjectile
    {
        private int bounceCount = 0;
        private const int MaxBounces = 3;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prismatic Bolt"); // Not needed in 1.4+
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Rotate projectile
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Create dust based on bounce count
            Color dustColor = GetColorForBounce(bounceCount);
            int dustType = DustID.RainbowMk2;

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);
            dust.velocity *= 0.3f;
            dust.noGravity = true;
            dust.scale = 1.2f;
            dust.color = dustColor;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounceCount++;
            
            // Play bounce sound
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // Increase damage with each bounce
            Projectile.damage = (int)(Projectile.damage * 1.1f);

            // Handle bouncing physics
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -oldVelocity.X;
            
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -oldVelocity.Y;

            // Visual effects for bounce
            for (int i = 0; i < 5; i++)
            {
                Color dustColor = GetColorForBounce(bounceCount);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
                dust.noGravity = true;
                dust.scale = 1.5f;
                dust.color = dustColor;
                dust.velocity *= 2f;
            }

            // Destroy after max bounces
            if (bounceCount >= MaxBounces)
                Projectile.Kill();

            return false;
        }

        private Color GetColorForBounce(int bounce)
        {
            switch (bounce)
            {
                case 0:
                    return new Color(255, 0, 0); // Red
                case 1:
                    return new Color(0, 255, 0); // Green
                case 2:
                    return new Color(0, 0, 255); // Blue
                default:
                    return new Color(255, 255, 255); // White
            }
        }
    }
}