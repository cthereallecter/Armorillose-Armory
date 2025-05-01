// v0.2.0.0
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace Armorillose.Content.Projectiles
{
    /// <summary>
    /// A bouncing slime ball projectile that loses damage over time.
    /// Used by the Slime Slinger weapon.
    /// </summary>
    public class SlimeBallProjectile : ModProjectile
    {
        // Constants
        private const int MAX_BOUNCES = 4;
        private const float DAMAGE_REDUCTION_PER_BOUNCE = 0.12f; // 12% damage reduction per bounce
        private const float BOUNCE_VELOCITY_MULTIPLIER = 0.8f;
        
        // Fields
        private int _bounceCount = 0;

        public override void SetStaticDefaults()
        {
            // No special defaults needed
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; // Will bounce until MAX_BOUNCES is reached
            Projectile.timeLeft = 600; // 10 seconds
            Projectile.alpha = 50; // Slight transparency
            Projectile.light = 0.1f; // Small light emission
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1; // Moves at 60fps instead of 30fps
            
            // Use basic projectile physics
            Projectile.aiStyle = 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            _bounceCount++;
            
            // Create bounce dust effect
            CreateImpactDust(5);
            
            // Play bounce sound
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            
            // Calculate new velocity after bounce (with slightly diminished speed)
            HandleBouncePhysics(oldVelocity);
            
            // After MAX_BOUNCES, destroy the projectile
            if (_bounceCount >= MAX_BOUNCES)
            {
                // Create a slime splash effect on final impact
                CreateImpactDust(15);
                Projectile.Kill();
            }
            
            return false; // Don't destroy on tile collision (handled manually)
        }
        
        private void HandleBouncePhysics(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X * BOUNCE_VELOCITY_MULTIPLIER;
            }
            
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y * BOUNCE_VELOCITY_MULTIPLIER;
            }
        }
        
        private void CreateImpactDust(int dustCount)
        {
            for (int i = 0; i < dustCount; i++)
            {
                Dust.NewDust(
                    Projectile.position, 
                    Projectile.width, 
                    Projectile.height, 
                    DustID.t_Slime, 
                    Projectile.velocity.X * 0.25f, 
                    Projectile.velocity.Y * 0.25f);
            }
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Reduce projectile damage for subsequent hits
            Projectile.damage = (int)(Projectile.damage * (1f - DAMAGE_REDUCTION_PER_BOUNCE));
            
            // Create slime particle effect on hit
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(
                    target.position, 
                    target.width, 
                    target.height, 
                    DustID.t_Slime, 
                    Projectile.velocity.X * 0.2f, 
                    Projectile.velocity.Y * 0.2f);
            }
        }
        
        public override void AI()
        {
            // Projectile rotation based on velocity
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