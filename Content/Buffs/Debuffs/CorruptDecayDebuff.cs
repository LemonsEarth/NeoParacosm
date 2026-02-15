namespace NeoParacosm.Content.Buffs.Debuffs;

public class CorruptDecayDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}

public class CorruptDecayPlayer : ModPlayer
{
    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff(BuffType<CorruptDecayDebuff>()))
        {
            Player.DOTDebuff(14);
        }
    }

    public override void PostUpdateRunSpeeds()
    {
        if (Player.HasBuff(BuffType<CorruptDecayDebuff>()))
        {
            Player.runAcceleration *= 0.5f;
        }
    }
}
