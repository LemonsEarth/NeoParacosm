using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class ManaGuardBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {

    }
}

public class ManaGuardPlayer : ModPlayer
{
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Player.HasBuff(BuffType<ManaGuardBuff>()))
        {
            Player.CheckMana(info.SourceDamage, true, true);
            Player.manaRegenDelay = 120;
        }
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (Player.HasBuff(BuffType<ManaGuardBuff>()))
        {
            float manaPercent = (float)Player.statMana / Player.statManaMax2;
            float bonusDamage = 1 - manaPercent - 0.5f;
            bonusDamage = MathHelper.Clamp(bonusDamage, -0.2f * Player.GetElementalExpertiseBoost(SpellElement.Pure), 0.5f);
            modifiers.FinalDamage *= (1 + bonusDamage);
        }
    }
}
