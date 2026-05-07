using NeoParacosm.Core.UI.ResearcherUI.Note;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Materials;

public class BoneBiterHead : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 28;
        Item.value = Item.sellPrice(0, 8, 0, 0);
        Item.rare = ItemRarityID.Pink;
    }
}

public class BoneBiterHeadDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.SandsharkCorrupt;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<BoneBiterHead>(), 8));
    }
}
