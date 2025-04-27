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
    /// <summary>
    /// A channeled beam projectile that fires a continuous eye laser.
    /// Used by the Eye Beam Staff weapon.
    /// </summary>
    public class EyeBeamProjectile : ModProjectile
    {
        // Constants
        private const float MAX_BEAM_LENGTH = 2000f;
        private const int BEAM_WIDTH = 32;
        private const int DUST_SPAWN_RATE = 2;
        private const float MANA_COST_PER_SECOND = 6f;

        // Properties to track beam state
        private Vector2 _beamStart;
        private Vector2 _beamEnd;
        private float _beamLength;
        private float _manaDrainTimer = 0f;
        
        public override void SetStaticDefaults()
        {
            // No special defaults needed
        }
        
        public override void SetDefaults()
        {
            Projectile.width = BEAM_WIDTH;
            Projectile.height = BEAM_WIDTH;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1; // Infinite penetration
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60; // Base time
            Projectile.aiStyle = -1; // Custom AI
            Projectile.alpha = 255; // Start fully transparent
        }
        
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Custom collision detection for the beam
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), 
                targetHitbox.Size(), 
                _beamStart, 
                _beamEnd, 
                BEAM_WIDTH, 
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
                _manaDrainTimer++;
                if (_manaDrainTimer >= 60) // Once per second
                {
                    player.CheckMana((int)MANA_COST_PER_SECOND, true);
                    _manaDrainTimer = 0;
                }
                
                // Calculate beam start position
                Vector2 playerHandPos = player.MountedCenter;
                playerHandPos.Y -= 6; // Adjust to match player's hand position
                
                // Calculate beam direction and rotation
                Vector2 beamDirection = Vector2.Normalize(Main.MouseWorld - playerHandPos);
                player.ChangeDir(beamDirection.X > 0 ? 1 : -1);
                
                // Update projectile position to follow player
                Projectile.position = playerHandPos;
                Projectile.velocity = beamDirection * 15f; // Keep velocity updated for rotation
                
                // Set the beam start position
                _beamStart = playerHandPos;
                
                // Custom raycast to find where beam hits
                float rayLength = MAX_BEAM_LENGTH;
                for (float i = 0; i < rayLength; i += 4f)
                {
                    Vector2 beamPos = _beamStart + beamDirection * i;
                    
                    // Check for tile collision
                    if (Collision.SolidCollision(beamPos, 1, 1))
                    {
                        rayLength = i;
                        break;
                    }
                }
                
                // Calculate the beam end position
                _beamEnd = _beamStart + beamDirection * rayLength;
                _beamLength = rayLength;
                
                // Create dust particles along the beam
                SpawnBeamDust(beamDirection, rayLength);
                
                // Play continuous sound occasionally
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
        
        private void SpawnBeamDust(Vector2 beamDirection, float rayLength)
        {
            for (float i = 0; i < rayLength; i += DUST_SPAWN_RATE)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 dustPos = _beamStart + beamDirection * i;
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
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            // Draw the beam
            if (_beamLength > 0)
            {
                // Prepare the beam texture
                Texture2D beamTexture = ModContent.Request<Texture2D>("Armorillose/Content/Projectiles/EyeBeamProjectile").Value;
                
                // Calculate beam drawing parameters
                Vector2 beamDirection = _beamEnd - _beamStart;
                float rotation = beamDirection.ToRotation() - MathHelper.PiOver2;
                
                // Draw beam segments
                float scaleY = _beamLength / beamTexture.Height;
                Color beamColor = new Color(255, 50, 50, 150);
                
                // Draw the beam
                Main.EntitySpriteDraw(
                    beamTexture,
                    _beamStart - Main.screenPosition,
                    new Rectangle(0, 0, beamTexture.Width, beamTexture.Height),
                    beamColor,
                    rotation,
                    new Vector2(beamTexture.Width * 0.5f, 0),
                    new Vector2(1f, scaleY),
                    SpriteEffects.None,
                    0);
            }
            
            return false; // Don't draw the projectile normally
        }
        
        public override void CutTiles()
        {
            // Allow the beam to cut things like grass
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(_beamStart, _beamEnd, BEAM_WIDTH, DelegateMethods.CutTiles);
        }
    }
}