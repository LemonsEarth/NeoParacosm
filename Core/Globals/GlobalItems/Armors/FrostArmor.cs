using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class FrostHelmettSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.FrostHelmet;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Ice, SpellBoostType.Expertise)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Ice, 0.25f);
    }
}

public class FrostBreastplateSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.FrostBreastplate;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Ice, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Ice, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, 0.15f);
    }
}

public class FrostLeggingsSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.FrostLeggings;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Ice, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Ice, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, 0.10f);
    }
}