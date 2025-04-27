using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Armorillose.Content.Items.Materials
{
    public class DemonEyeLens : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 35);  // 35 copper sell price
            Item.rare = ItemRarityID.Blue;            // Blue rarity (early game)
            Item.material = true;                     // Marks it as a crafting material
        }

        // Add a subtle glow effect
        public override void PostUpdate()
        {
            // Emit light
            Lighting.AddLight(Item.Center, 0.5f, 0.1f, 0.1f); // Red glow
            
            // Occasionally spawn particles
            if (Main.rand.NextBool(25))
            {
                Dust.NewDust(Item.position, Item.width, Item.height, DustID.RedTorch, 0f, 0f, 0, default, 0.5f);
            }
        }
    }
}