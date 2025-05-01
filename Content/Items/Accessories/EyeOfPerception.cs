// v0.2.0.0
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Items.Materials;

namespace Armorillose.Content.Items.Accessories
{
    public class EyeOfPerception : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(silver: 55);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Increase crit chance by 3%
            player.GetCritChance(DamageClass.Generic) += 3;
            
            // Add hunter effect (shows enemies on minimap)
            player.detectCreature = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Materials.DemonEyeLens>(), 15)
                .AddIngredient(ItemID.Lens, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}