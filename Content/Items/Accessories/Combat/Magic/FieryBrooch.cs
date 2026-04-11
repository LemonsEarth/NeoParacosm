using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class FieryBrooch : ModItem
{
    readonly float fireSpellDamageBoost = 15f;
    readonly float fireSpellExpertiseBoost = 20f;
    readonly int underworldDefenseBoost = 10;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(fireSpellDamageBoost, fireSpellExpertiseBoost, underworldDefenseBoost);
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 3);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, fireSpellDamageBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, fireSpellExpertiseBoost / 100f);
        if (player.ZoneUnderworldHeight)
        {
            player.statDefense += underworldDefenseBoost;
        }
    }
}

public class FieryBroochDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.VoodooDemon
            || entity.type == NPCID.Demon;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.VoodooDemon)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<FieryBrooch>(), 10));
        }
        else if (npc.type == NPCID.Demon)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<FieryBrooch>(), 50));
        }
    }
}
