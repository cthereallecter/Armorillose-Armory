using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Armorillose.Content.Items.Materials
{
    public class DemonEyeLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 35);
            Item.rare = ItemRarityID.Blue;
            Item.material = true;
        }

        public override void PostUpdate()
        {
            // Emit light
            Lighting.AddLight(Item.Center, 0.5f, 0.1f, 0.1f); // Red glow
            
            // Occasionally spawn particles
            if (Main.rand.NextBool(25))
            {
                Dust.NewDust(Item.position, Item.width, Item.height, 
                    DustID.RedTorch, 0f, 0f, 0, default, 0.5f);
            }
        }
    }
}