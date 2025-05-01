// v0.2.0.3
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

using Armorillose.Content.Projectiles;

namespace Armorillose.Content.Buffs
{
    public class EchoDetonatorBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Echo Detonators");
            // Description.SetDefault("Explosive detonators will blow up at your command");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Check if any detonators exist
            bool hasDetonators = false;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<EchoDetonatorProjectile>())
                {
                    hasDetonators = true;
                    break;
                }
            }

            if (!hasDetonators)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}