using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class SpectreMaskSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.SpectreMask;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Pure, SpellBoostType.Damage),
        (SpellElement.Holy, SpellBoostType.Damage),
        (SpellElement.Dark, SpellBoostType.Damage)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Pure, 0.40f);
        player.AddElementalDamageBoost(SpellElement.Holy, 0.30f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.30f);
    }
}

public class SpectreHoodSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.SpectreHood;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Pure, SpellBoostType.Expertise),
        (SpellElement.Holy, SpellBoostType.Expertise),
        (SpellElement.Dark, SpellBoostType.Expertise)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.40f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, 0.30f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.30f);
    }
}

public class SpectreRobeSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.SpectreRobe;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Pure, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Pure, 0.20f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.20f);
        player.AddElementalDamageBoost(SpellElement.Holy, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.15f);
    }
}

public class SpectrePantsSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.SpectrePants;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Pure, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Pure, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Holy, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, 0.10f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.10f);
    }
}