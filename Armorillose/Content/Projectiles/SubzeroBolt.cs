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

namespace Armorillose.Content.Projectiles
{
    public class SubzeroBolt : ModProjectile
    {
        // Remember enemies hit by this bolt for the slowing effect tracking
        public static readonly int SlowDuration = 180; // 3 seconds

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Subzero Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 0;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Rotate projectile
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Create dust trail
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.IceTorch,
                    0f, 0f, 100, default, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Apply frozen buff (1 second)
            target.AddBuff(BuffID.Frozen, 60);

            // Mark target as slowed for tracking
            target.GetGlobalNPC<SubzeroNPC>().ApplySlow(SlowDuration);

            // Visual effect
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(target.Center, DustID.IceTorch, speed * 5, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            // Create impact effect
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, speed * 3, Scale: 1.2f);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
        }
    }
}