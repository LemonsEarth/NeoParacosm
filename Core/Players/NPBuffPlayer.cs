using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;

namespace NeoParacosm.Core.Players;

public class NPBuffPlayer : ModPlayer
{
    public override void ResetEffects()
    {

    }

    public override void PostUpdateBuffs()
    {

    }

    public override void UpdateEquips()
    {
        if (Player.HasBuff(ModContent.BuffType<CrimsonRotDebuff>()))
        {
            Player.statDefense -= 10 - (int)(((float)Player.statLife / Player.statLifeMax2) * 10) + 1;
        }

        if (Player.HasBuff(ModContent.BuffType<CrimsonSacrificeDebuff>()))
        {
            if (Player.numMinions > 0)
            {
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.minion && proj.owner == Player.whoAmI)
                    {
                        LemonUtils.DustCircle(proj.Center, 8, 8, DustID.Crimson, 2f);
                    }
                }
                LemonUtils.DustCircle(Player.Center, 8, 8, DustID.Crimson, 2f);
            }
            Player.maxMinions = 0;
            Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Player.getRect()), DustID.Crimson).noGravity = true;
        }
    }

    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff(ModContent.BuffType<CrimsonRotDebuff>()))
        {
            DOTDebuff(20);
        }

        if (Player.HasBuff<CrimsonSacrificeCooldown>() && !Player.HasBuff(ModContent.BuffType<CrimsonSacrificeDebuff>()))
        {
            Player.lifeRegen -= 2;
        }
    }

    public override void UpdateLifeRegen()
    {
        if (Player.HasBuff(ModContent.BuffType<CrimsonSacrificeDebuff>()))
        {
            Player.lifeRegen += 60;
        }
    }

    public override void NaturalLifeRegen(ref float regen)
    {

    }

    void DOTDebuff(int damagePerSecond)
    {
        if (Player.lifeRegen > 0) Player.lifeRegen = 0;
        Player.lifeRegenTime = 0;
        Player.lifeRegen -= damagePerSecond * 2;
    }
}
