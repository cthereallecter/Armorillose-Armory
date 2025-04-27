using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using System;

namespace Armorillose.Content.Items.Weapons.Melee
{
    // Hellstone Inferno Blade - Fire melee weapon
    public class HellstoneInfernoBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hellstone Inferno Blade");
            // Tooltip.SetDefault("Ignites enemies with hellfire\n'Burn them all'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Orange; // Orange rarity fits the fire theme
            Item.value = Item.sellPrice(gold: 4, silver: 75);

            // Combat properties
            Item.damage = 38;
            Item.knockBack = 4f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18; // Slightly faster than the crystal blade
            Item.useAnimation = 18;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;

            // Sound
            Item.UseSound = SoundID.Item1;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Apply On Fire! debuff (vanilla)
            target.AddBuff(BuffID.OnFire, 300); // 5 seconds
            
            // Chance to create explosion effect
            if (Main.rand.NextBool(4)) // 1/4 chance
            {
                // Create fire explosion visual
                for (int i = 0; i < 15; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f);
                    Dust.NewDust(target.Center, 10, 10, DustID.Torch, speed.X, speed.Y);
                    
                    if (i % 5 == 0)
                    {
                        Dust.NewDust(target.Center, 10, 10, DustID.Flare, speed.X, speed.Y);
                    }
                }
                
                // Play explosion sound
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
                
                // Bonus damage
                int explosionDamage = Item.damage / 2;
                target.life -= explosionDamage;
                target.HitEffect();
                
                if (target.life <= 0 && !target.immortal)
                    target.checkDead();
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // Fire dust while swinging
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(
                    new Vector2(hitbox.X, hitbox.Y),
                    hitbox.Width,
                    hitbox.Height,
                    DustID.Torch,
                    player.direction * 2,
                    0,
                    Scale: 1.0f
                );
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.HellstoneBar, 15);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
}