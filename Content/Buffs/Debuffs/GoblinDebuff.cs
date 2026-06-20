namespace NeoParacosm.Content.Buffs.Debuffs;

public class GoblinDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}

public class GoblinDebuffNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (player.HasBuff(BuffType<GoblinDebuff>()))
        {
            spawnRate /= 2;
            maxSpawns *= 2;
        }
    }
}
