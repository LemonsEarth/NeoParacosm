namespace NeoParacosm.Content.Buffs.Debuffs;

public class CrimsonSacrificeDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetDamage(DamageClass.Generic) += 35f / 100f;
        player.GetAttackSpeed(DamageClass.Generic) += 20f / 100f;
    }
}
