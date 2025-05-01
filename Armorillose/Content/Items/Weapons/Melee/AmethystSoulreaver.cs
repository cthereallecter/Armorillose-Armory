using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using System;

namespace Armorillose.Content.Items.Weapons.Melee
{
    // Amethyst Crystal Sword - Melee weapon with crystal shards
    public class AmethystSoulreaver : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Amethyst Soulreaver");
            // Tooltip.SetDefault("Releases ethereal crystal shards when striking enemies\n'Crystallized essence of ancient spirits'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1.15f;
            Item.rare = ItemRarityID.LightPurple; // Matches amethyst color theme
            Item.value = Item.sellPrice(gold: 5, silver: 50);

            // Combat properties
            Item.damage = 45;
            Item.knockBack = 5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;

            // Sound
            Item.UseSound = SoundID.Item1;

            // Visual properties
            Item.shoot = ProjectileID.None; // No default projectile
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Create crystal shards on hit
            if (Main.rand.NextBool(3)) // 1/3 chance per hit
            {
                Vector2 velocity = target.Center - player.Center;
                velocity.Normalize();
                velocity *= 8f;
                
                // Spawn 1-3 crystal shards in a spread pattern
                int numShards = Main.rand.Next(1, 4);
                for (int i = 0; i < numShards; i++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                    int projType = ProjectileID.CrystalShard;
                    int damage = (int)(Item.damage * 0.5f); // Shards do 50% of the sword's damage
                    
                    Projectile.NewProjectile(
                        player.GetSource_ItemUse(Item),
                        target.Center,
                        newVelocity,
                        projType,
                        damage,
                        Item.knockBack / 2,
                        player.whoAmI
                    );
                }
                
                // Play crystal sound
                SoundEngine.PlaySound(SoundID.Item27, target.Center);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // Create dust effects while swinging
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(
                    new Vector2(hitbox.X, hitbox.Y),
                    hitbox.Width,
                    hitbox.Height,
                    DustID.GemAmethyst,
                    Scale: 1.2f
                );
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.GoldBar, 12);
            recipe.AddIngredient(ItemID.Amethyst, 15);
            recipe.AddIngredient(ItemID.SoulofLight, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
            
            // Alternate recipe with platinum
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.PlatinumBar, 12);
            recipe2.AddIngredient(ItemID.Amethyst, 15);
            recipe2.AddIngredient(ItemID.SoulofLight, 8);
            recipe2.AddTile(TileID.MythrilAnvil);
            recipe2.Register();
        }
    }
}