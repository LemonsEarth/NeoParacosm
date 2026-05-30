using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Materials;

public class FleshReaverHead : ModItem
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

public class FleshReaverHeadDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.SandsharkCrimson;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<FleshReaverHead>(), 8));
    }
}
