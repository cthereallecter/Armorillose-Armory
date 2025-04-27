using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Armorillose.Content.Items.Materials
{
    public class ZombieBrainFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 125;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 30);
            Item.rare = ItemRarityID.Blue;
            Item.material = true;
        }

        public override void PostUpdate()
        {
            // Occasionally spawn blood particles for "alive" effect
            if (Main.rand.NextBool(30))
            {
                Dust.NewDust(Item.position, Item.width, Item.height, -
                    DustID.Blood, 0f, 0f, 0, default, 0.8f);
            }
            
            // Add subtle pulsing light
            float pulseIntensity = 0.2f + (float)System.Math.Sin(Main.GameUpdateCount * 0.05f) * 0.1f;
            Lighting.AddLight(Item.Center, pulseIntensity * 0.3f, 0f, 0f);
        }
    }
}