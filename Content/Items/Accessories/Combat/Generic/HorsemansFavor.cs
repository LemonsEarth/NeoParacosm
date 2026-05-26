using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class HorsemansFavor : ModItem
{
    readonly float damageBoost = 8f;
    readonly float critBoost = 12f;
    readonly float drBoost = 10f;
    readonly float moveSpeedBoost = 18f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critBoost, drBoost, moveSpeedBoost);
    public override void SetDefaults()
    {
        Item.width = 56;
        Item.height = 56;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.mount != null && player.mount.Active && !player.mount.CanFly())
        {
            player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
            player.GetCritChance(DamageClass.Generic) += critBoost;
            player.endurance += drBoost / 100f;
            player.moveSpeed += moveSpeedBoost / 100f;
        }
    }
}

public class HorsemansFavorNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.HeadlessHorseman;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsPumpkinMoon(), ItemType<HorsemansFavor>(), 10));
    }
}