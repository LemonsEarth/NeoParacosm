using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class ArcheryRing : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 38;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddBuff(BuffID.Archery, 2);
    }
}

public class ArcheryRingDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GoblinArcher;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<ArcheryRing>(), 50, 1, 1));
    }
}
