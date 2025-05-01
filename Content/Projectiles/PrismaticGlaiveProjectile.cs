// v0.2.0.3
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using System;
using Terraria.Audio;

namespace Armorillose.Content.Projectiles
{
    public class PrismaticGlaiveProjectile : ModProjectile
    {
        private int bounceCount = 0;
        private const int MaxBounces = 3;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prismatic Bolt"); // Not needed in 1.4+
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Rotate projectile
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Create dust based on bounce count
            Color dustColor = GetColorForBounce(bounceCount);
            int dustType = DustID.RainbowMk2;

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);
            dust.velocity *= 0.3f;
            dust.noGravity = true;
            dust.scale = 1.2f;
            dust.color = dustColor;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounceCount++;

            // Play bounce sound
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // Increase damage with each bounce
            Projectile.damage = (int)(Projectile.damage * 1.1f);

            // Handle bouncing physics
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -oldVelocity.X;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -oldVelocity.Y;

            // Visual effects for bounce
            for (int i = 0; i < 5; i++)
            {
                Color dustColor = GetColorForBounce(bounceCount);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2);
                dust.noGravity = true;
                dust.scale = 1.5f;
                dust.color = dustColor;
                dust.velocity *= 2f;
            }

            // Destroy after max bounces
            if (bounceCount >= MaxBounces)
                Projectile.Kill();

            return false;
        }

        private Color GetColorForBounce(int bounce)
        {
            switch (bounce)
            {
                case 0:
                    return new Color(255, 0, 0); // Red
                case 1:
                    return new Color(0, 255, 0); // Green
                case 2:
                    return new Color(0, 0, 255); // Blue
                default:
                    return new Color(255, 255, 255); // White
            }
        }
    }
}