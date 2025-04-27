using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Items.Materials;
using Armorillose.Content.Projectiles;

namespace Armorillose.Content.Items.Weapons
{
    public class SlimeSlinger : ModItem
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            // Common weapon properties
            Item.width = 32;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 1, copper: 50);

            // Combat properties
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.autoReuse = true;
            Item.noMelee = true; // The weapon itself doesn't deal damage

            // Shooting properties
            Item.shoot = ModContent.ProjectileType<Projectiles.SlimeBallProjectile>();
            Item.shootSpeed = 4.5f;
            Item.UseSound = SoundID.Item98; // Squishing slime sound
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Items.Materials.CongealedSlimeCore>(), 8)
                .AddIngredient(ItemID.Wood, 15)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}