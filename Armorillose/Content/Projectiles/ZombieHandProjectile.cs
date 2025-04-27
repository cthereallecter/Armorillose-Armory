using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Armorillose.Content.Projectiles
{
    /// <summary>
    /// A throwable zombie hand projectile that extends, steals life on hit, and retracts back to the player.
    /// Used by the Zombie Hand weapon.
    /// </summary>
    public class ZombieHandProjectile : ModProjectile
    {
        // Constants
        private const float MAX_EXTEND_DISTANCE = 300f;
        private const int LIFESTEAL_AMOUNT = 8; // HP recovered per hit
        private const float RETRACT_SPEED = 15f;
        private const float RETRACT_DELAY = 20f; // Ticks before retracting
        private const float CLOSE_DISTANCE = 20f; // Distance to player to kill projectile

        // Fields
        private bool _initialized = false;
        private Vector2 _initialPosition;
        private float _maxDistance = 0f;
        private float _retractTimer = 0f;
        private bool _isRetracting = false;
        private int _targetNPC = -1;

        public override void SetStaticDefaults()
        {
            // No special defaults needed
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1; // Can hit multiple enemies
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600; // 10 seconds max life
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1; // Moves at 60fps instead of 30fps
            Projectile.aiStyle = -1; // Custom movement - don't use vanilla AI styles
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Initialize on first update
            if (!_initialized)
            {
                _initialPosition = Projectile.position;
                SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position); // Fleshy sound on extend
                _initialized = true;
            }

            // Keep the player from using other items while hand is extended
            player.itemAnimation = player.itemTime = 2;
            player.itemRotation = (float)Math.Atan2(
                Projectile.velocity.Y * Projectile.direction, 
                Projectile.velocity.X * Projectile.direction);

            // Calculate current distance from player
            Vector2 playerCenter = player.MountedCenter;
            float currentDistance = Vector2.Distance(playerCenter, Projectile.Center);

            // Handle extension and retraction logic
            HandleMovement(player, playerCenter, currentDistance);

            // Rotate projectile based on its velocity
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            // Create dust trail effect
            CreateTrailEffect();
        }

        private void HandleMovement(Player player, Vector2 playerCenter, float currentDistance)
        {
            // Set max distance if not already set
            if (_maxDistance == 0f)
            {
                _maxDistance = Math.Min(
                    Vector2.Distance(playerCenter, playerCenter + Projectile.velocity * 20f), 
                    MAX_EXTEND_DISTANCE);
            }

            if (!_isRetracting)
            {
                // Check if hand reached max distance or hit a tile
                if (currentDistance >= _maxDistance || Projectile.velocity.Length() < 0.1f)
                {
                    BeginRetraction();
                }
            }
            else
            {
                // Retraction logic
                _retractTimer += 1f;

                // Retract starts after a brief delay
                if (_retractTimer >= RETRACT_DELAY)
                {
                    Vector2 direction = playerCenter - Projectile.Center;
                    direction.Normalize();

                    // Gradually increase retraction speed
                    float speed = Math.Min(RETRACT_SPEED, RETRACT_SPEED * (_retractTimer - RETRACT_DELAY) / RETRACT_DELAY);
                    Projectile.velocity = direction * speed;

                    // Check if hand returned to player
                    if (currentDistance < CLOSE_DISTANCE)
                    {
                        Projectile.Kill();
                    }
                }
            }
        }

        private void BeginRetraction()
        {
            _isRetracting = true;
            _retractTimer = 0f;
            _targetNPC = -1; // Reset target when retracting
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
        }

        private void CreateTrailEffect()
        {
            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Blood,
                    0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            // Lifesteal effect
            PerformLifesteal(player);

            // Visual effect for lifesteal
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.Blood,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f);
                dust.noGravity = true;
            }

            // Force retract after hitting an enemy
            if (!_isRetracting)
            {
                BeginRetraction();
                _targetNPC = target.whoAmI; // Remember which NPC we hit
            }

            // Playing a hit sound
            SoundEngine.PlaySound(SoundID.NPCHit18, target.position);
        }

        private void PerformLifesteal(Player player)
        {
            if (player.statLife < player.statLifeMax2)
            {
                player.statLife += LIFESTEAL_AMOUNT;
                player.HealEffect(LIFESTEAL_AMOUNT);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Start retracting if we hit a tile
            BeginRetraction();

            // Play sound effect
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            // Create dust effect
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(
                    Projectile.position, 
                    Projectile.width, 
                    Projectile.height,
                    DustID.Blood,
                    oldVelocity.X * 0.2f,
                    oldVelocity.Y * 0.2f);
            }

            return false; // Don't destroy on tile collision
        }

        // Draw a chain between player and hand
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 distToProj = playerCenter - center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();

            // Draw the chain
            DrawChain(playerCenter, center, projRotation, distance, lightColor);

            return true;
        }
        
        private void DrawChain(Vector2 playerCenter, Vector2 center, float projRotation, float distance, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Armorillose/Content/Projectiles/ZombieHandChain").Value;

            int segmentIndex = 0; // To alternate rotation

            while (distance > 30f)
            {
                distance -= 16f;
                Vector2 drawPos = center + (playerCenter - center) * distance / (playerCenter - center).Length();

                // Alternate rotation between 90 degrees and 270 degrees
                float rotation = (segmentIndex % 2 == 0) ? MathHelper.PiOver2 : MathHelper.PiOver2 * 3f;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos - Main.screenPosition,
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    lightColor,
                    rotation,
                    new Vector2(texture.Width * 0.5f, texture.Height * 0.5f),
                    1f,
                    SpriteEffects.None,
                    0);

                segmentIndex++;
            }
        }
    }
}