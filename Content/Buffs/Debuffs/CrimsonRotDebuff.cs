namespace NeoParacosm.Content.Buffs.Debuffs;

public class CrimsonRotDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}

public class CrimsonRotPlayer : ModPlayer
{
    public override void UpdateEquips()
    {
        if (Player.HasBuff(BuffType<CrimsonRotDebuff>()))
        {
            Player.statDefense -= 10 - (int)(((float)Player.statLife / Player.statLifeMax2) * 10) + 1;
        }
    }

    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff(BuffType<CrimsonRotDebuff>()))
        {
            Player.DOTDebuff(20);
        }
    }
}

public class CrimsonRotNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (npc.HasBuff(BuffType<CrimsonRotDebuff>()))
        {
            modifiers.Defense.Flat -= (10 - (npc.life / npc.lifeMax * 10));
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (npc.HasBuff(BuffType<CrimsonRotDebuff>()))
        {
            npc.DOTDebuff(24, ref damage);
        }
    }
}
