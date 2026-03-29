using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class StormflowerBud : ModItem
{
    readonly float lightningExpertiseBoost = 20f;
    readonly float natureDamageBoost = 15f;
    readonly float natureExpertiseBoost = 15f;
    readonly float movementSpeedBoost = 10f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(natureDamageBoost, natureExpertiseBoost, lightningExpertiseBoost, movementSpeedBoost);
    public override void SetDefaults()
    {
        Item.width = 44;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddElementalExpertiseBoost(SpellElement.Lightning, lightningExpertiseBoost / 100f);
        player.AddElementalDamageBoost(SpellElement.Nature, natureDamageBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Nature, natureExpertiseBoost / 100f);
        player.moveSpeed += movementSpeedBoost / 100f;
    }
}

public class SunflowerBudDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Hornet
            || entity.type == NPCID.JungleBat
            || entity.type == NPCID.JungleSlime
            || entity.type == NPCID.SpikedJungleSlime
            || entity.type == NPCID.ManEater
            || entity.type == NPCID.Snatcher
            || entity.type == NPCID.MossHornet
            || entity.type == NPCID.GiantTortoise
            || entity.type == NPCID.Derpling;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(Condition.Thunderstorm.ToDropCondition(ShowItemDropInUI.Always), ItemType<StormflowerBud>(), 50));
    }
}
