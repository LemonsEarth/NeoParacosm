using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class ApprenticeHatSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ApprenticeHat;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Pure, SpellBoostType.Expertise)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.20f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.20f);
    }
}

public class ApprenticeRobeSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ApprenticeRobe;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Pure, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, 0.20f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.10f);
        player.AddElementalDamageBoost(SpellElement.Pure, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.15f);
    }
}

public class ApprenticeTrousersSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.ApprenticeTrousers;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Pure, SpellBoostType.Damage)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.10f);
        player.AddElementalDamageBoost(SpellElement.Pure, 0.15f);
    }
}