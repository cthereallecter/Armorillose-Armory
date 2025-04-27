using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace Armorillose.Content.Projectiles
{
    public class SlimeBallProjectile : ModProjectile
    {
        // Track number of bounces
        private int bounceCount = 0;
        private const int MaxBounces = 4;
        private const float DamageReductionPerBounce = 0.12f; // 12% damage reduction per bounce

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; // Will bounce until MaxBounces is reached
            Projectile.timeLeft = 600; // 10 seconds
            Projectile.alpha = 50; // Slight transparency
            Projectile.light = 0.1f; // Small light emission
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1; // Moves at 60fps instead of 30fps
            
            // Custom bounce behavior
            Projectile.aiStyle = 1; // Throws projectile style for initial behavior
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounceCount++;
            
            // Create bounce dust effect
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.t_Slime, 
                    Projectile.velocity.X * 0.1f, 
                    Projectile.velocity.Y * 0.1f);
            }
            
            // Play bounce sound
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            
            // Calculate new velocity after bounce (with slightly diminished speed)
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }
            
            // After MaxBounces, destroy the projectile
            if (bounceCount >= MaxBounces)
            {
                // Create a slime splash effect on final impact
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                        DustID.t_Slime, 
                        oldVelocity.X * 0.25f, 
                        oldVelocity.Y * 0.25f);
                }
                Projectile.Kill();
            }
            
            return false; // Don't destroy on tile collision (handled manually)
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Reduce projectile damage for subsequent hits
            Projectile.damage = (int)(Projectile.damage * (1f - DamageReductionPerBounce));
            
            // Create slime particle effect on hit
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, 
                    DustID.t_Slime, 
                    Projectile.velocity.X * 0.2f, 
                    Projectile.velocity.Y * 0.2f);
            }
        }
        
        public override void AI()
        {
            // Projectile rotation
            Projectile.rotation += 0.2f * Projectile.velocity.X * 0.1f;
            
            // Trail effect
            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position, 
                    Projectile.width, 
                    Projectile.height, 
                    DustID.t_Slime, 
                    0f, 0f, 100, Color.White, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }
    }
}