using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Armorillose.Content.Items.Materials;
using Armorillose.Content.Projectiles;

namespace Armorillose.Content.Items.Weapons
{
    public class ZombieHand : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            // Basic properties
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;

            // Combat properties
            Item.damage = 14;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 3f;
            Item.autoReuse = true;

            // Use properties
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.UseSound = SoundID.Item1;

            // Projectile properties
            Item.shoot = ModContent.ProjectileType<Projectiles.ZombieHandProjectile>();
            Item.shootSpeed = 12f;
            Item.noMelee = true; // The projectile will do the damage, not the swing
            Item.noUseGraphic = true; // Don't show the item when used
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Items.Materials.ZombieBrainFragment>(), 10)
                .AddIngredient(ItemID.IronBar, 8) // Iron bar variant
                .AddTile(TileID.Anvils)
                .Register();

            // Add lead variant recipe
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Items.Materials.ZombieBrainFragment>(), 10)
                .AddIngredient(ItemID.LeadBar, 8) // Lead bar variant
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}