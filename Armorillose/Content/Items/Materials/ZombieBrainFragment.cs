using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Armorillose.Content.Items.Materials
{
    public class ZombieBrainFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
           
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 30);  // 30 copper sell price
            Item.rare = ItemRarityID.Blue;            // Blue rarity (early game)
            Item.material = true;                     // Marks it as a crafting material
        }

        // Custom pulsing effect to make it feel "alive"
        public override void PostUpdate()
        {
            // Occasionally spawn blood particles
            if (Main.rand.NextBool(30))
            {
                Dust.NewDust(Item.position, Item.width, Item.height, DustID.Blood, 0f, 0f, 0, default, 0.8f);
            }
        }
    }
}