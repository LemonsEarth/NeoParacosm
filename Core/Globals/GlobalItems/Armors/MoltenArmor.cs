using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class MoltenHelmetSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.MoltenHelmet;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [(SpellElement.Fire, SpellBoostType.Expertise)];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.15f);
    }
}

public class MoltenBreastplateSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.MoltenBreastplate;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Fire, SpellBoostType.Both)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.10f);
    }
}

public class MoltenGreavesSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.MoltenGreaves;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [(SpellElement.Fire, SpellBoostType.Expertise)];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.10f);
    }
}