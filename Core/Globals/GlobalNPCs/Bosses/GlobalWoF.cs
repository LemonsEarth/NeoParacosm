using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems;
using Terraria.DataStructures;

namespace NeoParacosm.Core.Globals.GlobalNPCs.Bosses;

public class GlobalWoF : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.WallofFlesh;
    }

    public override void OnKill(NPC npc)
    {

    }
}
