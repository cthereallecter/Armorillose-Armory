// v0.2.0.3
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Projectiles;

namespace Armorillose.Content.Items.Weapons
{
    public class SubzeroCatalyst : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Subzero Catalyst");
            /* Tooltip.SetDefault("Fires an ice bolt that slows enemies" +
                "\nEnemies killed while slowed explode into damaging ice shards"); */
            Item.staff[Item.type] = true; // This makes the item use a staff animation
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item28;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SubzeroBolt>();
            Item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FrostCore, 12)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient(ItemID.CrystalShard, 15)
                .AddIngredient(ItemID.IceFeather)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

}