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
        (SpellElement.Nature, SpellBoostType.Both),
        (SpellElement.Earth, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddGlobalElementalDamageBoost(0.25f);
        player.AddGlobalElementalExpertiseBoost(0.25f);
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
        (SpellElement.Nature, SpellBoostType.Both),
        (SpellElement.Earth, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddGlobalElementalDamageBoost(0.25f);
        player.AddGlobalElementalExpertiseBoost(0.25f);
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
        (SpellElement.Nature, SpellBoostType.Both),
        (SpellElement.Earth, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddGlobalElementalDamageBoost(0.25f);
        player.AddGlobalElementalExpertiseBoost(0.25f);
    }
}