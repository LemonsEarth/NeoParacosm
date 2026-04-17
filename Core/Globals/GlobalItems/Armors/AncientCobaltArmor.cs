using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class AncientCobaltHelmetSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientCobaltHelmet;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [(SpellElement.Earth, SpellBoostType.Expertise)];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.15f);
    }
}

public class AncientCobaltBreastplateSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientCobaltBreastplate;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Earth, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Earth, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.05f);
    }
}

public class AncientCobaltLeggingsSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientCobaltLeggings;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [(SpellElement.Earth, SpellBoostType.Expertise)];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.1f);
    }
}
