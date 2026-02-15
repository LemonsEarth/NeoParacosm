using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class RageGauntlet : ModItem
{
    readonly float damageBoost = 20f;
    readonly float critBoost = 20f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critBoost);
    public override void SetDefaults()
    {
        Item.width = 42;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 3);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.GetLifePercent() <= 0.1f)
        {
            player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
            player.GetCritChance(DamageClass.Generic) += critBoost;
        }
    }
}

public class BigMimicNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.BigMimicCorruption || entity.type == NPCID.BigMimicCrimson || entity.type == NPCID.BigMimicHallow;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<RageGauntlet>(), 10, 1, 1));
    }
}
