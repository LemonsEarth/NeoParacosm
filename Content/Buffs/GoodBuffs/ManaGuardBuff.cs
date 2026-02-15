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
        player.statDefense += (int)(10 * MathF.Pow(player.GetElementalExpertiseBoost(BaseSpell.SpellElement.Pure), 2));
        player.endurance += 15f / 100f;
        player.GetDamage(DamageClass.Generic) -= 10f / 100f;
    }
}

public class ManaGuardPlayer : ModPlayer
{
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Player.HasBuff(BuffType<ManaGuardBuff>()))
        {
            Player.CheckMana((info.Damage * 2) / LemonUtils.GetDifficulty(), true, true);
            Player.manaRegenDelay = 120;
        }
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (Player.HasBuff(BuffType<ManaGuardBuff>()))
        {
            float manaPercent = (float)Player.statMana / Player.statManaMax2;
            float bonusDamage = 1 - manaPercent;
            modifiers.FinalDamage *= (1 + bonusDamage);
        }
    }
}
