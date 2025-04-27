using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures; // Add this line to import PlayerDrawSet

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

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (hasZombiesTenacity && tenacityCooldown <= 0 && !tenacityActive)
            {
                // Activate the effect when hit
                tenacityActive = true;
                tenacityActiveTime = 180; // 3 seconds (60 ticks per second)
                tenacityCooldown = 600; // 10 seconds cooldown

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