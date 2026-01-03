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

    public override void Update(Player player, ref int buffIndex)
    {

    }
}
