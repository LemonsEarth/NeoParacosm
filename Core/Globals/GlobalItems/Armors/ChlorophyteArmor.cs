using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class ChlorophyteHeadgearSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ChlorophyteHeadgear;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Nature, SpellBoostType.Both)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Nature, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Nature, 0.25f);
    }
}

public class ChlorophytePlateMailSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ChlorophytePlateMail;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Nature, SpellBoostType.Both)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Nature, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Nature, 0.15f);
    }
}

public class ChlorophyteGreavesSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ChlorophyteGreaves;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [(SpellElement.Nature, SpellBoostType.Expertise)];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Nature, 0.15f);
    }
}