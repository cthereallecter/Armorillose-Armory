using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Armorillose.Content.Projectiles
{
    public class ZombieHandProjectile : ModProjectile
    {
        // Properties
        private const float MaxExtendDistance = 300f;
        private const int LifestealAmount = 8; // HP recovered per hit
        private const float RetractSpeed = 15f;

        // AI fields
        private bool initialized = false;
        private Vector2 initialPosition;
        private float maxDistance = 0f;
        private float retractTimer = 0f;
        private bool isRetracting = false;
        private int targetNPC = -1;

        public override void SetStaticDefaults()
        {
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

            // Custom movement - don't use vanilla ai styles
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Initialize on first update
            if (!initialized)
            {
                initialPosition = Projectile.position;
                SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position); // Fleshy sound on extend
                initialized = true;
            }

            // Keep the player from using other items while hand is extended
            player.itemAnimation = player.itemTime = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            // Calculate current distance from player
            Vector2 playerCenter = player.MountedCenter;
            float currentDistance = Vector2.Distance(playerCenter, Projectile.Center);

            // Set max distance if not already set
            if (maxDistance == 0f)
            {
                maxDistance = Math.Min(Vector2.Distance(playerCenter, playerCenter + Projectile.velocity * 20f), MaxExtendDistance);
            }

            // Manage extension and retraction
            if (!isRetracting)
            {
                // Check if hand reached max distance or hit a tile
                if (currentDistance >= maxDistance || Projectile.velocity.Length() < 0.1f)
                {
                    isRetracting = true;
                    retractTimer = 0f;
                    targetNPC = -1; // Reset target when retracting

                    // Slight pause at full extension
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                // Retraction logic
                retractTimer += 1f;

                // Retract starts after a brief delay
                if (retractTimer >= 20f)
                {
                    Vector2 direction = playerCenter - Projectile.Center;
                    direction.Normalize();

                    // Gradually increase retraction speed
                    float speed = Math.Min(RetractSpeed, RetractSpeed * (retractTimer - 20f) / 20f);
                    Projectile.velocity = direction * speed;

                    // Check if hand returned to player
                    if (currentDistance < 20f)
                    {
                        Projectile.Kill();
                    }
                }
            }

            // Rotate projectile based on its velocity
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            // Create dust trail effect
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
            if (player.statLife < player.statLifeMax2)
            {
                player.statLife += LifestealAmount;
                player.HealEffect(LifestealAmount);
            }

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
            if (!isRetracting)
            {
                isRetracting = true;
                retractTimer = 0f;
                targetNPC = target.whoAmI; // Remember which NPC we hit
                Projectile.netUpdate = true;
            }

            // Playing a hit sound
            SoundEngine.PlaySound(SoundID.NPCHit18, target.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Start retracting if we hit a tile
            isRetracting = true;
            retractTimer = 0f;
            Projectile.velocity = Vector2.Zero;

            // Play sound effect
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            // Create dust effect
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Blood,
                    oldVelocity.X * 0.2f,
                    oldVelocity.Y * 0.2f);
            }

            return false; // Don't destroy on tile collision
        }

        // Optional: Draw a chain/rope between player and hand
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 distToProj = playerCenter - center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();

            // Draw the chain
            while (distance > 30f)
            {
                // Get texture for the chain
                Texture2D texture = ModContent.Request<Texture2D>("Armorillose/Content/Projectiles/ZombieHandChain").Value;

                // Position of the chain segment
                distance -= 16f;
                Vector2 drawPos = center + distToProj * distance / distToProj.Length();

                // Draw the chain segment
                Main.EntitySpriteDraw(
                    texture,
                    drawPos - Main.screenPosition,
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    lightColor,
                    projRotation,
                    new Vector2(texture.Width * 0.5f, texture.Height * 0.5f),
                    1f,
                    SpriteEffects.None,
                    0);
            }

            return true;
        }
    }
}