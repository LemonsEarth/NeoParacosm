using NeoParacosm.Content.NPCs.Hostile.Corruption;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Movement;

public class CutWyvernSegment : ModItem
{
    float flightTimeBoost = 25f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(flightTimeBoost);
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.wingTimeMax = (int)(player.wingTimeMax * (1 + flightTimeBoost / 100f));
    }
}

public class CutWyvernSegmentDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.WyvernHead;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<CutWyvernSegment>(), 10));

    }
}
