using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems;
using Terraria.DataStructures;

namespace NeoParacosm.Core.Globals.GlobalNPCs.Bosses;

public class GlobalEvilBoss : GlobalNPC
{
    bool spawnedNPC = false;
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.BrainofCthulhu
            || entity.type == NPCID.EaterofWorldsHead || entity.type == NPCID.EaterofWorldsBody || entity.type == NPCID.EaterofWorldsTail;
    }

    public override void OnKill(NPC npc)
    {
        if (NPC.downedBoss2 || !npc.boss || spawnedNPC || ResearcherQuest.DragonRemainsTileEntityPos == Point16.Zero) return;

        spawnedNPC = true;

        Vector2 remainsPos = ResearcherQuest.DragonRemainsTileEntityPos.ToWorldCoordinates();

        if (LemonUtils.NotClient())
        {
            NPC.NewNPCDirect(npc.GetSource_FromThis(), remainsPos + new Vector2(500, 0), NPCType<Researcher>());
        }
        if (ResearcherQuest.Progress == ResearcherQuest.ProgressState.NotDownedEvilBoss)
        {
            ResearcherQuest.Progress = ResearcherQuest.ProgressState.DownedEvilBoss;
        }
    }
}
