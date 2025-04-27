using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Items.Materials;

namespace Armorillose.Content.Items.Accessories
{
    public class SlimeHeart : ModItem
    {
        public override void SetStaticDefaults()
        {   
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Apply the enhanced regeneration effect when below 50% health
            if (player.statLife <= player.statLifeMax2 / 2)
            {
                player.lifeRegen += 4; // +2 HP/sec = +4 lifeRegen (lifeRegen is measured in half-HP per second)
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Materials.CongealedSlimeCore>(), 15)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}