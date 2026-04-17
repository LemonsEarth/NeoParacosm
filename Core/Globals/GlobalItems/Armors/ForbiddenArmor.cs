using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class ForbiddenMaskSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientBattleArmorHat;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Earth, SpellBoostType.Both)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Earth, 0.12f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.20f);
    }
}

public class ForbiddenRobesSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientBattleArmorShirt;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
         (SpellElement.Earth, SpellBoostType.Both)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Earth, 0.18f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.15f);
    }
}

public class ForbiddenTreadsSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientBattleArmorPants;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Earth, SpellBoostType.Both)
       ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Earth, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.15f);
    }
}