using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Armorillose.Content.Items.Materials;
using Armorillose.Content.Projectiles;

namespace Armorillose.Content.Items.Weapons
{
    public class EyeBeamStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true; // This makes the item rotate correctly when used
        }

        public override void SetDefaults()
        {
            // Basic properties
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(silver: 3);
            Item.rare = ItemRarityID.Blue;

            // Combat properties
            Item.damage = 9; // Damage per tick
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4; // Mana cost per use
            Item.knockBack = 0.5f;

            // Use properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item157; // Laser-like sound

            // Projectile properties
            Item.shoot = ModContent.ProjectileType<Projectiles.EyeBeamProjectile>();
            Item.shootSpeed = 20f;
            Item.noMelee = true; // The projectile will do the damage, not the swing
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Items.Materials.DemonEyeLens>(), 12)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }

        // Special behavior: Add channel functionality for continuous beam
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.EyeBeamProjectile>()] < 1;
        }
    }
}