namespace NeoParacosm.Content.Buffs.Debuffs;

public class DeathflameDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        Dust.NewDustPerfect(npc.RandomPos(), DustID.GemDiamond, -Vector2.UnitY * Main.rand.NextFloat(4, 6), Scale: Main.rand.NextFloat(1, 2)).noGravity = true;
        Dust.NewDustPerfect(npc.RandomPos(), DustID.Ash, -Vector2.UnitY * Main.rand.NextFloat(2, 3), newColor: Color.Black, Scale: Main.rand.NextFloat(1, 2)).noGravity = true;
    }
}

public class DeathflamePlayer : ModPlayer
{
    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff(BuffType<DeathflameDebuff>()))
        {
            Player.DOTDebuff((int)(Player.statLifeMax2 * 0.07f));
        }
    }

    public override void UpdateEquips()
    {
        if (Player.HasBuff(BuffType<DeathflameDebuff>()))
        {
            Player.statDefense -= 15;
        }
    }
}

public class DeathflameNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (npc.HasBuff(BuffType<DeathflameDebuff>()))
        {
            float damagePerSecond = npc.lifeMax * 0.005f + 10;
            if (damagePerSecond > 50) damagePerSecond = 50;
            npc.DOTDebuff(damagePerSecond, ref damage);
        }
    }
}
