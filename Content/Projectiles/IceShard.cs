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
    public class IceShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Shard");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 0;
            Projectile.light = 0.2f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Rotate shard in the direction it's moving
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Create ice trail
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.IceTorch,
                    0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            // Slow down over time
            Projectile.velocity *= 0.99f;
        }

        public override void Kill(int timeLeft)
        {
            // Create shatter effect
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, speed * 2, Scale: 0.8f);
                d.noGravity = true;
            }
        }
    }
}