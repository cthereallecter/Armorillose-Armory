using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;

using Armorillose.Content.Players;

namespace Armorillose.Content.Items.Accessories
{
    public class VenomousEmbrace : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Venomous Embrace");
            /* Tooltip.SetDefault("Grants immunity to Poison" +
                "\nAttacks have a 15% chance to inflict Venom" +
                "\nIncreases damage and critical strike chance while in the Underground Jungle"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Poison immunity
            player.buffImmune[BuffID.Poisoned] = true;

            // Check if in Underground Jungle biome
            bool inUndergroundJungle = player.ZoneJungle && player.ZoneRockLayerHeight;
            if (inUndergroundJungle)
            {
                // Apply underground jungle bonus
                player.GetDamage(DamageClass.Generic) += 0.06f;
                player.GetCritChance(DamageClass.Generic) += 10f;

                // Visual effect in jungle
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(
                        player.position,
                        player.width,
                        player.height,
                        DustID.JungleSpore,
                        0f, 0f, 100, default, 1.2f);
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                }
            }

            // Add venom effect to player's OnHitNPC hook
            player.GetModPlayer<VenomousEmbracePlayer>().hasVenomousEmbrace = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bezoar)
                .AddIngredient(ItemID.JungleRose)
                .AddIngredient(ItemID.VialofVenom, 5)
                .AddIngredient(ItemID.ChlorophyteOre, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}