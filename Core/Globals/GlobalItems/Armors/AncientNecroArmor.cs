using NeoParacosm.Content.Items.Weapons.Magic.Spells;
namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class AncientNecroHelmetSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.AncientNecroHelmet;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Dark, SpellBoostType.Expertise)
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.25f);
    }
}
