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
            Item.shoot = ModContent.ProjectileType<EchoDetonatorProj>();
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
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<EchoDetonatorProj>() && proj.ai[1] == 0)
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

    public class EchoDetonatorProj : ModProjectile
    {
        // Constants for behavior
        private const int DetonationTime = 30;
        private const float HoverAmplitude = 6f;
        private const float HoverSpeed = 0.05f;
        
        // Properties for detonator
        private bool isDetonating => Projectile.ai[1] == 1;
        private float detonationTimer = 0;
        private float hoverOffset;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Echo Detonator");
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000; // 5 minutes
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            // Check if owner is still valid
            Player owner = Main.player[Projectile.owner];
            EchoDetonatorPlayer modPlayer = owner.GetModPlayer<EchoDetonatorPlayer>();
            
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }
            
            // Keep the buff active
            if (owner.HasBuff(ModContent.BuffType<EchoDetonatorBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            
            // Handle detonation sequence
            if (isDetonating)
            {
                HandleDetonation();
                return;
            }
            
            // Normal hovering behavior
            float hoverFactor = (float)Math.Sin((Main.GameUpdateCount + Projectile.whoAmI * 10) * HoverSpeed);
            hoverOffset = hoverFactor * HoverAmplitude;
            
            // Visual effects for idle state
            if (Main.rand.NextBool(15))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Electric,
                    0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }

        private void HandleDetonation()
        {
            // Process detonation delay
            if (Projectile.ai[2] > 0)
            {
                Projectile.ai[2]--;
                return;
            }
            
            // Increment detonation timer
            detonationTimer++;
            
            if (detonationTimer >= DetonationTime)
            {
                // Detonate!
                Detonate();
                return;
            }
            
            // Visual effects during countdown
            if (Main.rand.NextBool(3))
            {
                Color dustColor = Color.Lerp(Color.Yellow, Color.Red, detonationTimer / DetonationTime);
                
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch,
                    0f, 0f, 100, dustColor, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }
            
            // Pulsate size
            float pulseRate = 1f + (float)Math.Sin(detonationTimer * 0.5f) * 0.2f;
            Projectile.scale = pulseRate;
        }

        private void Detonate()
        {
            Player owner = Main.player[Projectile.owner];
            
            // Explosion sound
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            
            // Create explosion visuals
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                float scale = Main.rand.NextFloat(1f, 2f);
                float distance = Main.rand.NextFloat(10f, 40f);
                
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + speed * distance / 2,
                    DustID.Torch,
                    speed * Main.rand.NextFloat(2f, 6f),
                    Scale: scale);
                
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }
            
            // Create sound wave visuals
            for (int i = 0; i < 40; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                float scale = Main.rand.NextFloat(0.6f, 1.2f);
                float distance = Main.rand.NextFloat(50f, 100f);
                
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + speed * distance / 3,
                    DustID.BlueTorch,
                    speed * Main.rand.NextFloat(2f, 8f),
                    Scale: scale);
                
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }
            
            // Damage nearby enemies
            if (Main.myPlayer == Projectile.owner)
            {
                // Create the actual explosion damage area
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<EchoDetonatorExplosion>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner);
                
                // Apply speed buff to owner
                owner.GetModPlayer<EchoDetonatorPlayer>().AddSpeedBoost(300); // 5 seconds
            }
            
            // Kill the projectile
            Projectile.Kill();
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }

    public class EchoDetonatorExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DaybreakExplosion;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Echo Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; // Hit once
        }

        public override void AI()
        {
            // Do nothing, just wait for time to expire
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Use a circle collider for better explosion feeling
            return Collision.CheckAABBvCircle(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.width / 2);
        }
    }

    public class EchoDetonatorPlayer : ModPlayer
    {
        public int speedBoostTime = 0;

        public void AddSpeedBoost(int time)
        {
            // Add more time to the buff
            speedBoostTime = Math.Max(speedBoostTime, time);
        }

        public override void ResetEffects()
        {
            // Process speed boost
            if (speedBoostTime > 0)
            {
                // Apply speed boost
                Player.moveSpeed += 0.2f;
                
                // Visual effect
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.BlueTorch,
                        0f, -2f, 100, default, 0.8f);
                    dust.noGravity = true;
                    dust.velocity.X *= 0.3f;
                }
                
                // Decrease timer
                speedBoostTime--;
            }
        }
    }

    public class EchoDetonatorBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Echo Detonators");
            // Description.SetDefault("Explosive detonators will blow up at your command");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Check if any detonators exist
            bool hasDetonators = false;
            
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<EchoDetonatorProj>())
                {
                    hasDetonators = true;
                    break;
                }
            }
            
            if (!hasDetonators)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}