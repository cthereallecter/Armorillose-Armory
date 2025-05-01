using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Armorillose.Content.Items.Materials;
using Armorillose.Content.Players;

namespace Armorillose.Content.Items.Armor.Reanimated
{
    [AutoloadEquip(EquipType.Head)]
    public class ReanimatedHelmet : ModItem
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
            return body.type == ModContent.ItemType<ReanimatedChestplate>() && legs.type == ModContent.ItemType<ReanimatedGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<ArmorillosePlayer>().reanimatedArmorSet = true;
            player.setBonus = "Regenerate 2 HP every second during nighttime";
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ZombieBrainFragment>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}