// v0.2.0.0
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
        private const float MAX_EXTEND_DISTANCE = 300f;
        private const int LIFESTEAL_AMOUNT = 8;
        private const float RETRACT_SPEED = 15f;
        private const float RETRACT_DELAY = 20f;
        private const float CLOSE_DISTANCE = 20f;

        private bool _initialized = false;
        private Vector2 _initialPosition;
        private float _maxDistance = 0f;
        private float _retractTimer = 0f;
        private bool _isRetracting = false;
        private int _targetNPC = -1;
        private bool _screechEvent = false;

        public override void SetStaticDefaults() {}

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!_initialized)
            {
                _initialPosition = Projectile.position;
                SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position);
                _initialized = true;

                if (Main.rand.NextBool(100))
                {
                    _screechEvent = true;
                    SoundEngine.PlaySound(SoundID.ZombieMoan, Projectile.position);
                }
            }

            player.itemAnimation = player.itemTime = 2;
            player.itemRotation = (float)Math.Atan2(
                Projectile.velocity.Y * Projectile.direction,
                Projectile.velocity.X * Projectile.direction);

            Vector2 playerCenter = player.MountedCenter;
            float currentDistance = Vector2.Distance(playerCenter, Projectile.Center);

            HandleMovement(player, playerCenter, currentDistance);

            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            CreateTrailEffect();
        }

        private void HandleMovement(Player player, Vector2 playerCenter, float currentDistance)
        {
            if (_maxDistance == 0f)
            {
                _maxDistance = Math.Min(Vector2.Distance(playerCenter, playerCenter + Projectile.velocity * 20f), MAX_EXTEND_DISTANCE);
            }

            if (!_isRetracting)
            {
                if (currentDistance >= _maxDistance || Projectile.velocity.Length() < 0.1f)
                {
                    BeginRetraction();
                }
            }
            else
            {
                _retractTimer += 1f;

                if (_retractTimer >= RETRACT_DELAY)
                {
                    Vector2 direction = playerCenter - Projectile.Center;
                    direction.Normalize();

                    float speed = Math.Min(RETRACT_SPEED, RETRACT_SPEED * (_retractTimer - RETRACT_DELAY) / RETRACT_DELAY);
                    Projectile.velocity = direction * speed;

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
            _targetNPC = -1;
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
        }

        private void CreateTrailEffect()
        {
            if (Main.rand.NextBool(5))
            {
                int dustType = _screechEvent ? DustID.Ichor : DustID.Blood;
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    dustType,
                    0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            PerformLifesteal(player);

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

            if (!_isRetracting)
            {
                BeginRetraction();
                _targetNPC = target.whoAmI;
            }

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
            BeginRetraction();
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

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

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 distToProj = playerCenter - center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();

            DrawChain(playerCenter, center, projRotation, distance, lightColor);

            return true;
        }

        private void DrawChain(Vector2 playerCenter, Vector2 center, float projRotation, float distance, Color lightColor)
        {
            float wiggleSpeed = 0.1f;
            float segmentLength = 16f;
            float tension = GetChainTension();
            Texture2D texture = ModContent.Request<Texture2D>("Armorillose/Content/Projectiles/ZombieHandChain").Value;

            int segmentIndex = 0;

            while (distance > 30f)
            {
                distance -= segmentLength;
                Vector2 toPlayer = playerCenter - center;
                float rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 drawPos = center + toPlayer.SafeNormalize(Vector2.UnitX) * distance;

                float baseWiggle = (float)Math.Sin(Main.GameUpdateCount * wiggleSpeed + segmentIndex * 0.5f) * 0.1f;
                float wiggle = baseWiggle * (1f - tension);

                drawPos += toPlayer.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * wiggle * 16f;
                drawPos += toPlayer.SafeNormalize(Vector2.UnitX) * (tension * 4f);

                Main.EntitySpriteDraw(
                    texture,
                    drawPos - Main.screenPosition,
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    lightColor,
                    rotation,
                    new Vector2(texture.Width / 2f, texture.Height / 2f),
                    1f,
                    SpriteEffects.None,
                    0);

                segmentIndex++;
            }
        }

        private float GetChainTension()
        {
            if (_isRetracting)
            {
                if (_retractTimer >= RETRACT_DELAY)
                    return 1f;
                else
                    return MathHelper.Clamp(_retractTimer / RETRACT_DELAY, 0f, 1f);
            }
            return 0f;
        }
    }
}
