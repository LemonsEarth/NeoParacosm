using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using NeoParacosm.Core.Systems;
using Terraria.Audio;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Summoner
{
    [AutoloadEquip(EquipType.Head)]
    public class AscendedCrimsonHelmet : ModItem
    {
        static readonly float damageBoost = 5;
        static readonly int sentryBoost = 1;

        static readonly float setBonusDamage = 6;
        static readonly float setBonusNegDamage = 20;
        static readonly int setBonusMana = 40;
        static readonly int setBonusSentryBoost = 1;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, sentryBoost);
        public static LocalizedText setBonusText;

        int timer = 0;

        public override void SetStaticDefaults()
        {
            setBonusText = this.GetLocalization("SetBonus").WithFormatArgs(setBonusDamage, setBonusMana, setBonusSentryBoost, setBonusNegDamage);
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 28;
            Item.defense = 3;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += damageBoost / 100;
            player.maxTurrets += sentryBoost;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AscendedCrimsonScalemail>() && legs.type == ModContent.ItemType<AscendedCrimsonGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = setBonusText.Value;
            player.GetDamage(DamageClass.Generic) -= setBonusNegDamage / 100;
            player.GetDamage(DamageClass.Summon) += (setBonusNegDamage + setBonusDamage) / 100;
            player.statManaMax2 += setBonusMana;
            player.maxTurrets += setBonusSentryBoost;

            if (Main.myPlayer == player.whoAmI && KeybindSystem.CrimsonSacrifice.JustPressed && !player.HasBuff(ModContent.BuffType<CrimsonSacrificeCooldown>()))
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { PitchRange = (-0.2f, 0.2f) }, player.Center);
                player.AddBuff(ModContent.BuffType<CrimsonSacrificeDebuff>(), 600);
                player.AddBuff(ModContent.BuffType<CrimsonSacrificeCooldown>(), 3600);
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            timer++;
            LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.CrimsonHelmet, position, scale, timer, frame, spriteBatch, Color.Yellow);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            timer++;
            LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.CrimsonHelmet, rotation, scale, timer, spriteBatch, Color.Yellow);
            return false;
        }
    }
}
