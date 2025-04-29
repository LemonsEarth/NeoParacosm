using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;

namespace NeoParacosm.Core.Players;

public partial class NeoParacosmPlayer : ModPlayer
{
    public override void ResetEffects()
    {
        ResetAccessoryFields();
    }

    public override void PostUpdate()
    {
       
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (roundShield && !Player.HasBuff(ModContent.BuffType<KnockbackCooldown>()))
        {
            Player.AddBuff(ModContent.BuffType<KnockbackCooldown>(), 1800);
        }
    }
}
