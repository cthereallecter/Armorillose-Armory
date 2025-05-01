using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Buffs;
using Armorillose.Content.Players;
using Armorillose.Content.Projectiles;

namespace Armorillose.Content.Items.Weapons.Summon
{
    public class EchoDetonator : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Echo Detonator");
            /* Tooltip.SetDefault("Places explosive detonators that hover in place" +
                "\nRight-click to detonate them in sequence" +
                "\nExplosions create sound waves that increase your movement speed"); */
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // Allow full screen usage with gamepad
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true; // Item can target through walls
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(gold: 4, silver: 50);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<EchoDetonatorProjectile>();
            Item.shootSpeed = 0f; // Does not shoot projectile forward
            Item.buffType = ModContent.BuffType<EchoDetonatorBuff>();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true; // Allow right-click usage
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2) // Right click
            {
                // Trigger all detonators
                TriggerAllDetonators(player);
                return true;
            }
            
            return null; // Default behavior for left click
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Only handle normal left-click (place detonator)
            if (player.altFunctionUse == 2)
                return false;

            // Check if the maximum number of detonators is reached
            int detonatorCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == Item.shoot)
                {
                    detonatorCount++;
                }
            }

            if (detonatorCount >= 8)
            {
                // Find the oldest detonator and kill it
                int oldestProj = -1;
                int oldestTime = int.MaxValue;
                
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == player.whoAmI && proj.type == Item.shoot && proj.timeLeft < oldestTime)
                    {
                        oldestTime = proj.timeLeft;
                        oldestProj = i;
                    }
                }
                
                if (oldestProj >= 0)
                {
                    Main.projectile[oldestProj].Kill();
                }
            }

            // Create the detonator at the cursor position
            Vector2 spawnPos = Main.MouseWorld;
            
            // Spawn the detonator
            int projIndex = Projectile.NewProjectile(
                player.GetSource_ItemUse(Item),
                spawnPos,
                Vector2.Zero,
                type,
                damage,
                knockBack,
                player.whoAmI);

            // Set up the detonator order
            if (projIndex < Main.maxProjectiles)
            {
                Main.projectile[projIndex].ai[0] = detonatorCount + 1; // Set the detonation order
            }

            // Add the buff
            player.AddBuff(Item.buffType, 2);

            return false; // Prevent default behavior
        }

        private void TriggerAllDetonators(Player player)
        {
            // Gather all active detonators
            List<Projectile> detonators = new List<Projectile>();
            
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<EchoDetonatorProjectile>() && proj.ai[1] == 0)
                {
                    detonators.Add(proj);
                }
            }
            
            // Sort by order
            detonators.Sort((p1, p2) => p1.ai[0].CompareTo(p2.ai[0]));
            
            // Trigger detonation sequence
            for (int i = 0; i < detonators.Count; i++)
            {
                Projectile detonator = detonators[i];
                detonator.ai[1] = 1; // Mark for detonation
                detonator.ai[2] = i * 10; // Delay between detonations
            }
            
            // Play activation sound
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, player.Center);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient(ItemID.ExplosivePowder, 15)
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}