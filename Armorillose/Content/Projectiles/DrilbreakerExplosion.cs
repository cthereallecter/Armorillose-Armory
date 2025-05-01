// v0.2.0.3
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Armorillose.Content.Projectiles
{
    public class DrillbreakerExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SolarWhipSwordExplosion;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drillbreaker Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.tileCollide = false;
            Projectile.hide = true; // Important: otherwise explosion would show the original texture
            Projectile.alpha = 255; // Make it invisible
        }

        public override void AI()
        {
            // Do nothing, just wait for time to expire
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Use a circle collider for better explosion feeling
            return Collision.CheckAABBvCircle(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.width / 2);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Apply OnFire to enemies hit
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}