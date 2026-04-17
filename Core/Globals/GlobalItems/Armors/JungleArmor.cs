using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class JungleHatSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.JungleHat;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [(SpellElement.Nature, SpellBoostType.Expertise)];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Nature, 0.15f);
    }
}

public class JungleShirtSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.JungleShirt;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Nature, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Nature, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Nature, 0.05f);
    }
}

public class JunglePantsSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.JunglePants;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [(SpellElement.Nature, SpellBoostType.Expertise)];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Nature, 0.1f);
    }
}