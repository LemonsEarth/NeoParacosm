using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class AncientShadowHelmetSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientShadowHelmet;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Dark, 0.08f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.12f);
    }
}

public class AncientShadowScalemailSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientShadowScalemail;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Dark, 0.12f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.08f);
    }
}

public class AncientShadowGreavesSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientShadowGreaves;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Dark, 0.08f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.08f);
    }
}
