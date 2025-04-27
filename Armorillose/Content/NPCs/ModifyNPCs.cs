using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Items.Materials;
using Armorillose.Content.Items.Weapons;

namespace Armorillose.Content.NPCs
{
    public class ModifyNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Check if the NPC is any slime variant
            if (IsSlimeVariant(npc.type) ||
                (npc.aiStyle == NPCAIStyleID.Slime))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.CongealedSlimeCore>(), 2.5f, 1, 1)); // 1 in 3 Chance, Min: 1, Max: 1
                
            }

            // Check if the NPC is any zombie variant
            if (IsZombieVariant(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.ZombieBrainFragment>(), 2.5f, 1, 3)); // 1 in 3 Chance, Min: 1, Max: 1
            }

            if (IsDemonEyeVariant(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.DemonEyeLens>(), 2.5f, 1, 1)); // 1 in 3 Chance, Min: 1, Max: 1
            }

            if (npc.type == NPCID.KingSlime)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.CongealedSlimeCore>(), 1, 7, 15)); // 1 in 1 Chance, Min: 7, Max: 15
            }


            if ((npc.type == NPCID.EyeofCthulhu))
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.DemonEyeLens>(), 1, 3, 8));

            if ((npc.type == NPCID.Retinazer) ||
                (npc.type == NPCID.Spazmatism))
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.DemonEyeLens>(), 1, 5, 12));


        }

        // Helper method to identify if an NPC is a slime variant
        private bool IsSlimeVariant(int npcType)
        {
            return npcType == NPCID.BlueSlime ||
                   npcType == NPCID.GreenSlime ||
                   npcType == NPCID.RedSlime ||
                   npcType == NPCID.PurpleSlime ||
                   npcType == NPCID.YellowSlime;
        }

        // Helper method to identify if an NPC is a zombie variant
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

        // Helper method to identify if an NPC is a demon eye variant
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