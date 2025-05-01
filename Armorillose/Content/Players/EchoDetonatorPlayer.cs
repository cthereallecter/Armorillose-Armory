using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Armorillose.Content.Players
{
    public class EchoDetonatorPlayer : ModPlayer
    {
        public int speedBoostTime = 0;

        public void AddSpeedBoost(int time)
        {
            // Add more time to the buff
            speedBoostTime = Math.Max(speedBoostTime, time);
        }

        public override void ResetEffects()
        {
            // Process speed boost
            if (speedBoostTime > 0)
            {
                // Apply speed boost
                Player.moveSpeed += 0.2f;

                // Visual effect
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.BlueTorch,
                        0f, -2f, 100, default, 0.8f);
                    dust.noGravity = true;
                    dust.velocity.X *= 0.3f;
                }

                // Decrease timer
                speedBoostTime--;
            }
        }
    }
}
