using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Armorillose.Content.Players
{
    public class ArmorillosePlayer : ModPlayer
    {
        // Armor set flags
        public bool slimeArmorSet;
        public bool reanimatedArmorSet;
        public bool watcherArmorSet;
        
        private int regenTimer = 0;
        private const int REGEN_DELAY = 30; // 60 ticks = 1 second

        public override void ResetEffects()
        {
            slimeArmorSet = false;
            reanimatedArmorSet = false;
            watcherArmorSet = false;
        }

        public override void PostUpdateEquips()
        {
            // Slime Armor Set bonus - implemented in UpdateArmorSet for noFallDmg
            if (slimeArmorSet && Player.velocity.Y > 0)
            {
                // Add bouncing on enemies logic here
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly || !npc.chaseable) continue;

                    if (Player.Hitbox.Intersects(npc.Hitbox) && Player.velocity.Y > 0)
                    {
                        Player.velocity.Y = -10f; // Bounce up
                        Player.immune = true;
                        Player.immuneTime = 20;
                        
                        // Create a HitInfo structure for the NPC damage
                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                        {
                            Damage = Player.statDefense * 2,
                            Knockback = 0f,
                            HitDirection = 0
                        };
                        
                        npc.StrikeNPC(hitInfo);
                        break;
                    }
                }
            }

            // Reanimated Armor Set bonus
            if (reanimatedArmorSet && !Main.dayTime)
            {
                regenTimer++;
                if (regenTimer >= REGEN_DELAY)
                {
                    regenTimer = 0;
                    Player.statLife = Math.Min(Player.statLife + 1, Player.statLifeMax2);
                    Player.HealEffect(4);
                }
            }
            else
            {
                regenTimer = 0;
            }

            // Watcher Armor Set bonus
            if (watcherArmorSet && !Main.dayTime)
            {
                // Increased vision radius at night
                Player.nightVision = true;
                Player.dangerSense = true;
                
                // +10% ranged and magic damage at night
                Player.GetDamage(DamageClass.Ranged) += 0.1f;
                Player.GetDamage(DamageClass.Magic) += 0.15f; // Changed to 15% as per set bonus description
            }
        }
    }
}