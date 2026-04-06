using NeoParacosm.Content.Items.Accessories.Combat.Ranged;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class MechanicalArm : ModItem
{
    readonly float attackSpeedBoost = 20f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(attackSpeedBoost);
    public override void SetDefaults()
    {
        Item.width = 60;
        Item.height = 52;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 75);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetAttackSpeed(DamageClass.Melee) += attackSpeedBoost / 100f;
        player.GetAttackSpeed(DamageClass.Ranged) += attackSpeedBoost / 100f;
        player.pickSpeed -= 0.15f;
        player.tileSpeed += 0.15f;
    }
}

public class MechanicalArmShopNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Cyborg;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<MechanicalArm>(), Condition.DownedPlantera);
    }
}
