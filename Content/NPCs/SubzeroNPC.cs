// v0.2.0.3
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

using Armorillose.Content.Projectiles;

namespace Armorillose.Content.NPCs
{
    public class SubzeroNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int slowTimer;
        public bool isSlowed => slowTimer > 0;

        public void ApplySlow(int duration)
        {
            slowTimer = duration;
        }

        public override void ResetEffects(NPC npc)
        {
            // Decrease timer
            if (slowTimer > 0)
                slowTimer--;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            // Make slowed enemies look frozen
            if (isSlowed)
            {
                // Visual effect for slowed state
                if (Main.rand.NextBool(10))
                {
                    Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.IceTorch);
                    d.noGravity = true;
                    d.scale = 0.8f;
                }
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            // Enemies that are slowed take more knockback
            if (isSlowed)
                knockback *= 1.5f;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Enemies that are slowed take more knockback
            if (isSlowed)
                knockback *= 1.5f;
        }

        public override void OnKill(NPC npc)
        {
            // If enemy dies while slowed, create ice shard explosion
            if (isSlowed && !npc.friendly && npc.lifeMax > 5 && !npc.SpawnedFromStatue)
            {
                // Play ice break sound
                SoundEngine.PlaySound(SoundID.Item27, npc.Center);

                // Create ice explosion visual effect
                for (int i = 0; i < 30; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Dust d = Dust.NewDustPerfect(npc.Center, DustID.IceTorch, speed * Main.rand.NextFloat(2f, 8f), Scale: 1.5f);
                    d.noGravity = true;
                }

                // Spawn ice shards that damage enemies
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int numShards = Main.rand.Next(3, 6);
                    for (int i = 0; i < numShards; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2CircularEdge(8f, 8f);
                        int shardDamage = (int)(npc.lifeMax * 0.1f);
                        if (shardDamage < 20) shardDamage = 20;
                        if (shardDamage > 60) shardDamage = 60;

                        Projectile.NewProjectile(
                            Entity.GetSource_Death(),
                            npc.Center,
                            velocity,
                            ModContent.ProjectileType<IceShard>(),
                            shardDamage,
                            2f,
                            Main.myPlayer);
                    }
                }
            }
        }
    }
}