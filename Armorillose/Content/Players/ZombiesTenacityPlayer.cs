using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;


namespace Armorillose.Content.Players
{

    public class ZombiesTenacityPlayer : ModPlayer
    {
        public bool hasZombiesTenacity = false;
        public int tenacityCooldown = 0;
        public bool tenacityActive = false;
        public int tenacityActiveTime = 0;

        public override void ResetEffects()
        {
            // Reset the effect flag each frame so it only applies when the accessory is equipped
            hasZombiesTenacity = false;
        }

        public override void PostUpdateEquips()
        {
            // Decrease cooldown if it's active
            if (tenacityCooldown > 0)
                tenacityCooldown--;

            // Handle active invincibility time
            if (tenacityActive)
            {
                tenacityActiveTime--;
                if (tenacityActiveTime <= 0)
                    tenacityActive = false;
            }
        }

        // Let's use ModifyHurt instead of PreHurt
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (hasZombiesTenacity && tenacityCooldown <= 0 && !tenacityActive)
            {
                // Activate the effect when hit
                tenacityActive = true;
                tenacityActiveTime = 90; // 1.5 seconds (60 ticks per second)
                tenacityCooldown = 1200; // 20 seconds cooldown

                // Still take normal damage from this hit
            }
            else if (tenacityActive)
            {
                // If the effect is active, make the player immune
                modifiers.FinalDamage *= 0;
            }
        }

        // Add visual effect when the tenacity is active
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (tenacityActive)
            {
                // Add a slight glow effect when the tenacity is active
                r = 0.7f;
                g = 0.5f;
                b = 0.5f;
                fullBright = true;
            }
        }
    }
}