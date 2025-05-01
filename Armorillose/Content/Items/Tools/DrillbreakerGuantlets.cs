using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Armorillose.Content.Items.Tools
{
    public class DrillbreakerGauntlets : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drillbreaker Gauntlets");
            /* Tooltip.SetDefault("Functions as both a pickaxe and a melee weapon" +
                "\nHeavy attacks create small explosions that damage enemies" +
                "\nExplosions can break weaker blocks in a small area"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            
            // Pickaxe power (150%)
            Item.pick = 150;
            
            // Also acts as a weapon
            Item.noUseGraphic = false; // Show the item when used
            Item.noMelee = false; // The projectile will do damage
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // Create dust at the swing location
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Torch);
                dust.noGravity = true;
                dust.scale = 1.5f;
                dust.velocity *= 0.5f;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            // Create explosion effect on critical hits or with 20% chance
            if (crit || Main.rand.NextFloat() < 0.2f)
            {
                // Create explosion
                CreateExplosion(target.Center, player);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddIngredient(ItemID.Obsidian, 8)
                .AddIngredient(ItemID.SoulofMight, 12)
                .AddIngredient(ItemID.PowerGlove)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        private void CreateExplosion(Vector2 position, Player owner)
        {
            // Play explosion sound
            SoundEngine.PlaySound(SoundID.Item14, position);

            // Create dust explosion effect
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                float scale = Main.rand.NextFloat(1f, 2.5f);
                float distance = Main.rand.NextFloat(30f, 70f);
                
                Dust dust = Dust.NewDustPerfect(
                    position + speed * distance / 2,
                    DustID.Torch,
                    speed * Main.rand.NextFloat(2f, 8f),
                    Scale: scale);
                
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }

            // Only destroy blocks on the owner's client or in single player
            if (Main.myPlayer == owner.whoAmI)
            {
                // Create explosion projectile for damaging enemies
                Projectile.NewProjectile(
                    owner.GetSource_ItemUse(owner.HeldItem),
                    position,
                    Vector2.Zero,
                    ModContent.ProjectileType<DrillbreakerExplosion>(),
                    (int)(owner.HeldItem.damage * 0.75f),
                    owner.HeldItem.knockBack,
                    owner.whoAmI);
                
                // Break weak blocks in a small area (only in PvE)
                if (!Main.invasionType.HasValue && !Main.pumpkinMoon && !Main.snowMoon && !Main.bloodMoon)
                {
                    DestroyTiles(position);
                }
            }
        }

        private void DestroyTiles(Vector2 center)
        {
            // Define the radius of block destruction
            int radius = 1;
            
            // Get the tile coordinates at the center
            int centerX = (int)(center.X / 16f);
            int centerY = (int)(center.Y / 16f);
            
            // Iterate through the surrounding tiles
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    // Skip if outside the world
                    if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                        continue;
                    
                    // Get the tile reference
                    Tile tile = Main.tile[x, y];
                    
                    // Skip if no tile is present
                    if (tile == null || !tile.HasTile)
                        continue;
                    
                    // Check if the tile is weak enough to be broken
                    // Only destroy weaker tiles like dirt, stone, clay, sand, etc.
                    if (Main.tileDungeon[tile.TileType] || 
                        TileID.Sets.Conversion.Sand[tile.TileType] || 
                        tile.TileType == TileID.Dirt || 
                        tile.TileType == TileID.Stone || 
                        tile.TileType == TileID.ClayBlock || 
                        tile.TileType == TileID.Sand || 
                        tile.TileType == TileID.Gravel)
                    {
                        // Break the tile
                        WorldGen.KillTile(x, y, false, false, false);
                        
                        // If in multiplayer, sync the changes
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                    }
                }
            }
        }
    }

    public class DrillbreakerExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SolarWhipSwordExplosion;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drillbreaker Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.tileCollide = false;
            Projectile.hide = true; // Important: otherwise explosion would show the original texture
            Projectile.alpha = 255; // Make it invisible
        }

        public override void AI()
        {
            // Do nothing, just wait for time to expire
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Use a circle collider for better explosion feeling
            return Collision.CheckAABBvCircle(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.width / 2);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Apply OnFire to enemies hit
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}