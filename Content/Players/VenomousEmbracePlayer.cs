// v0.2.0.3
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;

namespace Armorillose.Content.Players
{
    public class VenomousEmbracePlayer : ModPlayer
    {
        public bool hasVenomousEmbrace;

        public override void ResetEffects()
        {
            hasVenomousEmbrace = false;
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            ApplyVenomEffect(target);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (!proj.minion && !ProjectileID.Sets.IsAWhip[proj.type])
            {
                ApplyVenomEffect(target);
            }
        }

        private void ApplyVenomEffect(NPC target)
        {
            if (hasVenomousEmbrace && Main.rand.NextFloat() < 0.15f)
            {
                // Apply venom debuff (60 seconds)
                target.AddBuff(BuffID.Venom, 60 * 60);

                // Visual effect for venom
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        target.position,
                        target.width,
                        target.height,
                        DustID.Venom,
                        0f, 0f, 100, default, 1.2f);
                    dust.velocity *= 0.5f;
                    dust.noGravity = true;
                }
            }
        }
    }
}