using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Items.Materials;

namespace Armorillose.Content.NPCs
{
    /// <summary>
    /// Handles global NPC modifications, including drop rules for custom materials.
    /// </summary>
    public class ModifyNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Slime drops
            if (IsSlimeVariant(npc.type) || npc.aiStyle == NPCAIStyleID.Slime)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CongealedSlimeCore>(), 2, 1, 1));
            }

            // Zombie drops
            if (IsZombieVariant(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ZombieBrainFragment>(), 2, 1, 3));
            }

            // Demon Eye drops
            if (IsDemonEyeVariant(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonEyeLens>(), 2, 1, 1));
            }

            // Boss drops
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CongealedSlimeCore>(), 1, 7, 15));
                    break;
                case NPCID.EyeofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonEyeLens>(), 1, 3, 8));
                    break;
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonEyeLens>(), 1, 5, 12));
                    break;
            }
        }

        /// <summary>
        /// Determines if an NPC is a slime variant.
        /// </summary>
        private bool IsSlimeVariant(int npcType)
        {
            return npcType == NPCID.BlueSlime ||
                   npcType == NPCID.GreenSlime ||
                   npcType == NPCID.RedSlime ||
                   npcType == NPCID.PurpleSlime ||
                   npcType == NPCID.YellowSlime;
        }

        /// <summary>
        /// Determines if an NPC is a zombie variant.
        /// </summary>
        private bool IsZombieVariant(int npcType)
        {
            return npcType == NPCID.Zombie ||
                   npcType == NPCID.SmallZombie ||
                   npcType == NPCID.ArmedZombie || 
                   npcType == NPCID.BaldZombie ||  
                   npcType == NPCID.FemaleZombie ||     
                   npcType == NPCID.PincushionZombie ||   
                   npcType == NPCID.SlimedZombie || 
                   npcType == NPCID.SwampZombie || 
                   npcType == NPCID.TwiggyZombie;  
        }

        /// <summary>
        /// Determines if an NPC is a demon eye variant.
        /// </summary>
        private bool IsDemonEyeVariant(int npcType)
        {
            return npcType == NPCID.DemonEye ||
                   npcType == NPCID.DemonEye2 ||
                   npcType == NPCID.PurpleEye ||
                   npcType == NPCID.PurpleEye2 ||
                   npcType == NPCID.GreenEye ||
                   npcType == NPCID.GreenEye2 ||
                   npcType == NPCID.DialatedEye ||
                   npcType == NPCID.DialatedEye2 ||
                   npcType == NPCID.SleepyEye ||
                   npcType == NPCID.SleepyEye2 ||
                   npcType == NPCID.CataractEye ||
                   npcType == NPCID.CataractEye2 ||
                   npcType == NPCID.EyeballFlyingFish;
        }
    }
}