// v0.2.0.3
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

using Armorillose.Content.Players;

namespace Armorillose.Content.Projectiles
{
    public class EchoDetonatorProjectile : ModProjectile
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
}