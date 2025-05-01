using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Armorillose.Content.Items.Materials;

namespace Armorillose.Content.Items.Armor.Watcher
{
    [AutoloadEquip(EquipType.Legs)]
    public class WatcherGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DemonEyeLens>(), 15)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}