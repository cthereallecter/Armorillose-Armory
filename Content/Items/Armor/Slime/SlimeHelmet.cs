// v0.2.0.0
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Armorillose.Content.Items.Materials;
using Armorillose.Content.Players;

namespace Armorillose.Content.Items.Armor.Slime
{
    [AutoloadEquip(EquipType.Head)]
    public class SlimeHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 15);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SlimeChestplate>() && legs.type == ModContent.ItemType<SlimeGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<ArmorillosePlayer>().slimeArmorSet = true;
            player.noFallDmg = true;
            player.armorEffectDrawShadow = true;
            player.setBonus = "No fall damage and bounce on enemies when falling";
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CongealedSlimeCore>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}