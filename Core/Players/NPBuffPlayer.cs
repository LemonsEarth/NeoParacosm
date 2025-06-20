﻿using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using Terraria;

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
    }

    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff(ModContent.BuffType<CrimsonRotDebuff>()))
        {
            DOTDebuff(20);
        }
    }

    void DOTDebuff(int damagePerSecond)
    {
        if (Player.lifeRegen > 0) Player.lifeRegen = 0;
        Player.lifeRegenTime = 0;
        Player.lifeRegen -= damagePerSecond * 2;
    }
}
