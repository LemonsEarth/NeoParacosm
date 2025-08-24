namespace NeoParacosm.Content.Buffs.Debuffs.Cooldowns;

public class CrimsonSacrificeCooldown : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.HasBuff(BuffType<CrimsonSacrificeDebuff>()))
        {
            player.statDefense -= 10;
            player.moveSpeed -= 0.5f;
        }
    }
}
