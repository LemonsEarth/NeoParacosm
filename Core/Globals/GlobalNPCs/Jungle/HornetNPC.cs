using NeoParacosm.Content.NPCs.Hostile.Corruption;
using NeoParacosm.Content.NPCs.Hostile.Crimson;
using NeoParacosm.Content.NPCs.Hostile.Special;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using NeoParacosm.Core.Conditions;
using static Terraria.ID.NPCID;
using NeoParacosm.Content.Items.Weapons.Ranged;

namespace NeoParacosm.Core.Globals.GlobalNPCs.Jungle;

public class HornetNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override void SetStaticDefaults()
    {

    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && (entity.type == Hornet || entity.type == MossHornet);
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        ItemDropWithConditionRule rule1 = new ItemDropWithConditionRule(ItemType<TheGuardDuty>(), 50, 1, 1, new EyeOfCthulhuDowned());
        npcLoot.Add(rule1);
    }
}
