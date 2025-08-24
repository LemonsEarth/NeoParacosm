﻿using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using Terraria;

namespace NeoParacosm.Core.Players;

public class NPBuffPlayer : ModPlayer
{
    public bool grabbed { get; set; } = false;
    public bool fastFall { get; set; } = false;

    public override bool CanUseItem(Item item)
    {
        return !grabbed;
    }

    public override void ResetEffects()
    {
        if (fastFall)
        {
            Player.noKnockback = true;
            Player.blockExtraJumps = true;
            Player.maxFallSpeed *= 3;
            Player.velocity.Y = 20;
            Player.velocity.X = 0;
        }
        grabbed = false;
        fastFall = false;
    }

    public override void PostUpdateBuffs()
    {

    }

    public override void UpdateEquips()
    {
        if (Player.HasBuff(BuffType<CrimsonRotDebuff>()))
        {
            Player.statDefense -= 10 - (int)(((float)Player.statLife / Player.statLifeMax2) * 10) + 1;
        }

        if (Player.HasBuff(BuffType<DeathflameDebuff>()))
        {
            Player.statDefense -= 15;
        }

        if (Player.HasBuff(BuffType<CrimsonSacrificeDebuff>()))
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

    public override void PostUpdateEquips()
    {

    }

    public override void PostUpdateRunSpeeds()
    {
        if (Player.HasBuff(BuffType<CorruptDecayDebuff>()))
        {
            Player.runAcceleration *= 0.5f;
        }
    }

    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff(BuffType<CrimsonRotDebuff>()))
        {
            DOTDebuff(20);
        }

        if (Player.HasBuff(BuffType<CorruptDecayDebuff>()))
        {
            DOTDebuff(14);
        }

        if (Player.HasBuff(BuffType<DeathflameDebuff>()))
        {
            DOTDebuff((int)(Player.statLifeMax2 * 0.07f));
        }

        if (Player.HasBuff<CrimsonSacrificeCooldown>() && !Player.HasBuff(BuffType<CrimsonSacrificeDebuff>()))
        {
            Player.lifeRegen -= 2;
        }
    }

    public override void UpdateLifeRegen()
    {
        if (Player.HasBuff(BuffType<CrimsonSacrificeDebuff>()))
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
