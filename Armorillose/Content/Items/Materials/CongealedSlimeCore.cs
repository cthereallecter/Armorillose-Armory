// v0.2.0.0
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
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 25);
            Item.rare = ItemRarityID.Blue;
            Item.material = true;
        }

        public override void PostUpdate()
        {
            // Spawn slime particles occasionally for effect
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(Item.position, Item.width, Item.height, 
                    DustID.t_Slime, 0f, 0f, 0, Color.White, 1f);
            }
            
            // Add subtle glow
            Lighting.AddLight(Item.Center, 0.1f, 0.3f, 0.2f);
        }
    }
}