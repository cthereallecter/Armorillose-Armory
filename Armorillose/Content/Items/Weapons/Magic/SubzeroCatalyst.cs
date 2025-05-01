using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Armorillose.Content.Items.Weapons
{
    public class SubzeroCatalyst : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Subzero Catalyst");
            /* Tooltip.SetDefault("Fires an ice bolt that slows enemies" +
                "\nEnemies killed while slowed explode into damaging ice shards"); */
            Item.staff[Item.type] = true; // This makes the item use a staff animation
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item28;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SubzeroBolt>();
            Item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FrostCore, 12)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient(ItemID.CrystalShard, 15)
                .AddIngredient(ItemID.IceFeather)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SubzeroBolt : ModProjectile
    {
        // Remember enemies hit by this bolt for the slowing effect tracking
        public static readonly int SlowDuration = 180; // 3 seconds

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Subzero Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 0;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Rotate projectile
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Create dust trail
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.IceTorch,
                    0f, 0f, 100, default, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Apply frozen buff (1 second)
            target.AddBuff(BuffID.Frozen, 60);
            
            // Mark target as slowed for tracking
            target.GetGlobalNPC<SubzeroNPC>().ApplySlow(SlowDuration);
            
            // Visual effect
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(target.Center, DustID.IceTorch, speed * 5, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            // Create impact effect
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, speed * 3, Scale: 1.2f);
                d.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
        }
    }

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

    public class IceShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Shard");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 0;
            Projectile.light = 0.2f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Rotate shard in the direction it's moving
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Create ice trail
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.IceTorch,
                    0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
            
            // Slow down over time
            Projectile.velocity *= 0.99f;
        }

        public override void Kill(int timeLeft)
        {
            // Create shatter effect
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, speed * 2, Scale: 0.8f);
                d.noGravity = true;
            }
        }
    }
}