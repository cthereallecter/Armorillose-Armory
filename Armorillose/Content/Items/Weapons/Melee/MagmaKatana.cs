using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using System;
using Terraria.DataStructures;

namespace Armorillose.Content.Items.Weapons.Melee
{
    // Magma Katana - Ranged katana with fireballs
    public class MagmaKatana : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Magma Katana");
            // Tooltip.SetDefault("Fires a volley of molten projectiles\n'Cuts through the air like lava through stone'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1.1f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 50);

            // Combat properties
            Item.damage = 40;
            Item.knockBack = 3f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.DamageType = DamageClass.Ranged; // This is a ranged katana
            Item.autoReuse = true;
            
            // Projectile properties
            Item.shoot = ProjectileID.MolotovFire; // Will be visually overridden in shoot method
            Item.shootSpeed = 12f;
            Item.noMelee = true; // Important for ranged weapons - prevents melee damage

            // Sound
            Item.UseSound = SoundID.Item1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Determine projectile based on swing sequence
            // Alternate between different fire projectiles for visual variety
            int projType;
            
            switch (player.itemAnimation % 3)
            {
                case 0:
                    projType = ProjectileID.MolotovFire;
                    break;
                case 1:
                    projType = ProjectileID.Fireball;
                    break;
                default:
                    projType = ProjectileID.HellfireArrow;
                    break;
            }
            
            // Fire a spread of 3 projectiles
            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(15);
            
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                perturbedSpeed *= 1f - Main.rand.NextFloat(0.2f);
                
                Projectile.NewProjectile(
                    source,
                    position,
                    perturbedSpeed,
                    projType,
                    damage,
                    knockback,
                    player.whoAmI
                );
                
                // Create additional flames for visual effect
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(position, 10, 10, DustID.Torch, perturbedSpeed.X * 0.5f, perturbedSpeed.Y * 0.5f);
                }
            }
            
            return false; // Return false to prevent the original projectile from firing
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.HellstoneBar, 15);
            recipe.AddIngredient(ItemID.Obsidian, 12);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddIngredient(ItemID.LavaCharm, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}