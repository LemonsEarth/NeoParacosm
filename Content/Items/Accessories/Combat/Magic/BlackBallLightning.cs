using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class BlackBallLightning : ModItem
{
    readonly float lightningDarkDamageBoost = 15f;
    readonly float drBoost = 8f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(lightningDarkDamageBoost, drBoost);
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.endurance += drBoost / 100f;
        player.AddElementalDamageBoost(SpellElement.Lightning, lightningDarkDamageBoost / 100f);
        player.AddElementalDamageBoost(SpellElement.Dark, lightningDarkDamageBoost / 100f);
    }
}

public class BlackBallLightningDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCType<SpearKnight>()
            || entity.type == NPCType<ShieldKnight>()
            || entity.type == NPCType<StaffKnight>();
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<BlackBallLightning>(), 30));
    }
}
