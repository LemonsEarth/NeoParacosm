using NeoParacosm.Core.UI.ResearcherUI.Note;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Materials;

public class SwampThingsHead : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.value = Item.sellPrice(0, 3, 0, 0);
        Item.rare = ItemRarityID.Pink;
    }
}

public class SwampThingsHeadDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.SwampThing;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<SwampThingsHead>(), 100));
    }
}
