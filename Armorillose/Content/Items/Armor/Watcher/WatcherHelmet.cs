// v0.1.0.3
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Armorillose.Content.Items.Materials;
using Armorillose.Content.Players;

namespace Armorillose.Content.Items.Armor.Watcher
{
    [AutoloadEquip(EquipType.Head)]
    public class WatcherHelmet : ModItem
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

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WatcherChestplate>() && 
                   legs.type == ModContent.ItemType<WatcherGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<ArmorillosePlayer>().watcherArmorSet = true;
            player.setBonus = "Increased vision radius at night, +10% ranged and magic damage at night";
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DemonEyeLens>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}