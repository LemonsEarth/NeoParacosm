using NeoParacosm.Content.Buffs.Debuffs;
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
        if (Player.HasBuff(ModContent.BuffType<CrimsonRotDebuff>()))
        {
            Player.statDefense -= (10 - (Player.statLife / Player.statLifeMax2 * 10)); ;
        }
    }
}
