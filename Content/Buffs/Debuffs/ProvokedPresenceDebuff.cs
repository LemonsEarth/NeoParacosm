using NeoParacosm.Content.NPCs.Hostile.Special;

namespace NeoParacosm.Content.Buffs.Debuffs;

public class ProvokedPresenceDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}

public class ProvokedPresenceNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (player.HasBuff(BuffType<ProvokedPresenceDebuff>()) && !NPC.AnyNPCs(NPCType<Marauder>()))
        {
            spawnRate /= 3;
            maxSpawns *= 4;
        }
    }
}
