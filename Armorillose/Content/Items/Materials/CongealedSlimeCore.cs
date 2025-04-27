using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Armorillose.Content.Items.Materials
{
    public class CongealedSlimeCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 150;

            // If you want to use a custom glowing effect
            // ItemID.Sets.ItemIconPulse[Item.type] = true;
            // ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 25);  // 25 copper sell price
            Item.rare = ItemRarityID.Blue;            // Blue rarity (early game)
            Item.material = true;                     // Marks it as a crafting material
        }

        // Custom drawing effects if desired
        public override void PostUpdate()
        {
            // Optional: Spawn particles occasionally for a special effect
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(Item.position, Item.width, Item.height, DustID.t_Slime, 0f, 0f, 0, Color.White, 1f);
            }
        }
    }
}