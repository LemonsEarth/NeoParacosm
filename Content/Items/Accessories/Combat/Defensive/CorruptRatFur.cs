using NeoParacosm.Content.Items.Accessories.Combat.Magic;
using NeoParacosm.Content.NPCs.Hostile.Corruption;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class CorruptRatFur : ModItem
{
    readonly float drBoost = 15f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(drBoost);
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.GetLifePercent() >= 1f)
        {
            player.endurance += drBoost / 100f;
        }
    }
}

public class CorruptRatFurDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCType<DecayingRat>();
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<CorruptRatFur>(), 20));

    }
}
