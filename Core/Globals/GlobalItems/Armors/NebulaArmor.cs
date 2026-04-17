using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class NebulaHelmetSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.NebulaHelmet;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
       (SpellElement.Pure, SpellBoostType.Both),
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Ice, SpellBoostType.Both),
        (SpellElement.Lightning, SpellBoostType.Both),
        (SpellElement.Earth, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Pure, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Fire, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Ice, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Lightning, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Lightning, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Earth, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Holy, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.25f);
    }
}

public class NebulaBreastplateSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.NebulaBreastplate;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
       (SpellElement.Pure, SpellBoostType.Both),
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Ice, SpellBoostType.Both),
        (SpellElement.Lightning, SpellBoostType.Both),
        (SpellElement.Earth, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Pure, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Fire, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Ice, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Lightning, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Lightning, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Earth, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Holy, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.25f);
    }
}

public class NebulaLeggingsSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.NebulaLeggings;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
       (SpellElement.Pure, SpellBoostType.Both),
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Ice, SpellBoostType.Both),
        (SpellElement.Lightning, SpellBoostType.Both),
        (SpellElement.Earth, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Pure, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Fire, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Ice, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Lightning, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Lightning, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Earth, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Holy, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, 0.25f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.25f);
    }
}