using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.Enums;

namespace Armorillose.Content.Projectiles
{
    public class EyeBeamProjectile : ModProjectile
    {
        // Constants
        private const float MaxBeamLength = 2000f; // Maximum length of the beam
        private const int BeamWidth = 8; // Width of the beam
        private const int DustSpawnRate = 2; // How often dust particles spawn along the beam
        private const float ManaCostPerSecond = 10f; // Mana consumed per second while channeling

        // Properties to track beam state
        private Vector2 beamStart;
        private Vector2 beamEnd;
        private float beamLength;
        private float manaDrainTimer = 0f;
        
        public override void SetStaticDefaults()
        {
        }
        
        public override void SetDefaults()
        {
            Projectile.width = BeamWidth;
            Projectile.height = BeamWidth;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1; // Infinite penetration
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60; // Base time
            
            // Don't use vanilla AI styles
            Projectile.aiStyle = -1;
            Projectile.alpha = 255; // Start fully transparent
        }
        
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Custom collision detection for the beam
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), 
                targetHitbox.Size(), 
                beamStart, 
                beamEnd, 
                BeamWidth, 
                ref collisionPoint);
        }
        
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            
            // Keep beam alive as long as player channels and has mana
            if (player.channel && player.CheckMana(1, false))
            {
                Projectile.timeLeft = 5; // Reset time left while channeling
                
                // Drain mana continuously
                manaDrainTimer++;
                if (manaDrainTimer >= 60) // Once per second
                {
                    player.CheckMana((int)ManaCostPerSecond, true);
                    manaDrainTimer = 0;
                }
                
                // Calculate beam start position (from player center)
                Vector2 playerHandPos = player.MountedCenter;
                playerHandPos.Y -= 6; // Adjust to match player's hand position
                
                // Calculate beam direction and rotation
                Vector2 beamDirection = Vector2.Normalize(Main.MouseWorld - playerHandPos);
                player.ChangeDir(beamDirection.X > 0 ? 1 : -1);
                
                // Update projectile position to follow player
                Projectile.position = playerHandPos;
                Projectile.velocity = beamDirection * 15f; // Keep velocity updated for rotation
                
                // Set the beam start position
                beamStart = playerHandPos;
                
                // Custom raycast to find where beam hits
                float rayLength = MaxBeamLength;
                for (float i = 0; i < rayLength; i += 4f)
                {
                    Vector2 beamPos = beamStart + beamDirection * i;
                    
                    // Check for tile collision
                    if (Collision.SolidCollision(beamPos, 1, 1))
                    {
                        rayLength = i;
                        break;
                    }
                }
                
                // Calculate the beam end position
                beamEnd = beamStart + beamDirection * rayLength;
                beamLength = rayLength;
                
                // Create dust particles along the beam
                for (float i = 0; i < rayLength; i += DustSpawnRate)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 dustPos = beamStart + beamDirection * i;
                        Dust dust = Dust.NewDustDirect(
                            dustPos, 
                            1, 1, 
                            DustID.RedTorch, 
                            0f, 0f, 0, 
                            new Color(255, 50, 50), 
                            Main.rand.NextFloat(0.5f, 1.0f));
                        dust.noGravity = true;
                        dust.velocity = beamDirection.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(1f, 3f);
                    }
                }
                
                // Play continuous sound
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item33, Projectile.position);
                }
                
                // Make player point staff toward mouse
                player.itemRotation = (float)Math.Atan2(beamDirection.Y * player.direction, beamDirection.X * player.direction);
                
                // Ensure player can't use other items while channeling
                player.itemAnimation = player.itemTime = 2;
            }
            else
            {
                // End the beam if player stops channeling or runs out of mana
                Projectile.Kill();
            }
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            // Draw the beam
            if (beamLength > 0)
            {
                // Prepare the beam texture
                Texture2D beamTexture = ModContent.Request<Texture2D>("Armorillose/Content/Projectiles/EyeBeamProjectile").Value;
                Vector2 drawOrigin = new Vector2(beamTexture.Width * 0.5f, beamTexture.Height * 0.5f);
                
                // Calculate beam drawing parameters
                Vector2 beamDirection = beamEnd - beamStart;
                float rotation = beamDirection.ToRotation() - MathHelper.PiOver2;
                
                // Draw beam segments
                float scaleY = beamLength / beamTexture.Height;
                Color beamColor = new Color(255, 50, 50, 150);
                
                // Draw the beam
                Main.EntitySpriteDraw(
                    beamTexture,
                    beamStart - Main.screenPosition,
                    new Rectangle(0, 0, beamTexture.Width, beamTexture.Height),
                    beamColor,
                    rotation,
                    new Vector2(beamTexture.Width * 0.5f, 0),
                    new Vector2(1f, scaleY),
                    SpriteEffects.None,
                    0);
                
                // Draw glow effect at beam end
                // Texture2D glowTexture = ModContent.Request<Texture2D>("YourModNamespace/Projectiles/EyeBeamGlow").Value;
                // Main.EntitySpriteDraw(
                //     glowTexture,
                //     beamEnd - Main.screenPosition,
                //     new Rectangle(0, 0, glowTexture.Width, glowTexture.Height),
                //     beamColor,
                //     0f,
                //     new Vector2(glowTexture.Width * 0.5f, glowTexture.Height * 0.5f),
                //     1f,
                //     SpriteEffects.None,
                //     0);
            }
            
            return false; // Don't draw the projectile normally
        }
        
        public override void CutTiles()
        {
            // Allow the beam to cut things like grass
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(beamStart, beamEnd, BeamWidth, DelegateMethods.CutTiles);
        }
    }
}