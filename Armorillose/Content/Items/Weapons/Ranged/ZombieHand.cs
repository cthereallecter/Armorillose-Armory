using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Armorillose.Content.Items.Weapons.Ranged
{
    public class ZombieHand : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Basic properties
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;

            // Combat properties
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 3f;
            Item.autoReuse = true;

            // Use properties
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.UseSound = SoundID.Item1;

            // Projectile properties
            Item.shoot = ModContent.ProjectileType<Projectiles.ZombieHandProjectile>();
            Item.shootSpeed = 15f;
            Item.noMelee = true; // The projectile will do the damage, not the swing
            Item.noUseGraphic = true; // Don't show the item when used
        }

        public override void AddRecipes()
        {
            // Iron variant recipe
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Materials.ZombieBrainFragment>(), 15)
                .AddIngredient(ItemID.IronBar, 8)
                .AddTile(TileID.Anvils)
                .Register();

            // Lead variant recipe
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Materials.ZombieBrainFragment>(), 15)
                .AddIngredient(ItemID.LeadBar, 8)
                .AddTile(TileID.Anvils)
                .Register();

            // Tin variant recipe
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Materials.ZombieBrainFragment>(), 15)
                .AddIngredient(ItemID.TinBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}