using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Players;

namespace Armorillose.Content.Items.Accessories
{
    public class ZombiesTenacity : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(silver: 60);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Register our custom player effect
            player.GetModPlayer<ZombiesTenacityPlayer>().hasZombiesTenacity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Materials.ZombieBrainFragment>(), 15)
                .AddRecipeGroup("IronBar", 5) // Allows either Iron or Lead bars
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}