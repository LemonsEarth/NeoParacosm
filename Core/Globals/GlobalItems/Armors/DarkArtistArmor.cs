using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class DarkArtistHatSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ApprenticeAltHead;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Expertise)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.30f);
    }
}

public class DarkArtistRobeSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ApprenticeAltShirt;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, 0.25f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.20f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.30f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.30f);
    }
}

public class DarkArtistLeggingsSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ApprenticeAltPants;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Damage)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.20f);
    }
}