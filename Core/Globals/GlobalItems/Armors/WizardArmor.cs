using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public class WizardHatSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.WizardHat;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (SpellElement.Pure, SpellBoostType.Both),
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Ice, SpellBoostType.Both),
        (SpellElement.Lightning, SpellBoostType.Both),
        (SpellElement.Earth, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Pure, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.20f);
        player.AddElementalDamageBoost(SpellElement.Fire, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.20f);
        player.AddElementalDamageBoost(SpellElement.Ice, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, 0.20f);
        player.AddElementalDamageBoost(SpellElement.Lightning, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Lightning, 0.20f);
        player.AddElementalDamageBoost(SpellElement.Earth, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.20f);
        player.AddElementalDamageBoost(SpellElement.Holy, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, 0.20f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.15f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.20f);
    }
}

public class MagicHatSpellBoost : SpellBoostArmorPiece
{
    public override int ArmorPieceItemID => ItemID.MagicHat;

    public override (SpellElement, SpellBoostType)[] BoostedElements => [
       (SpellElement.Pure, SpellBoostType.Both),
        (SpellElement.Fire, SpellBoostType.Both),
        (SpellElement.Ice, SpellBoostType.Both),
        (SpellElement.Lightning, SpellBoostType.Both),
        (SpellElement.Earth, SpellBoostType.Both),
        (SpellElement.Holy, SpellBoostType.Both),
        (SpellElement.Dark, SpellBoostType.Both),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(SpellElement.Pure, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Fire, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Ice, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Lightning, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Lightning, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Earth, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Holy, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, 0.15f);
        player.AddElementalDamageBoost(SpellElement.Dark, 0.10f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, 0.15f);
    }
}

public abstract class RobeSpellBoost : SpellBoostArmorPiece
{
    public abstract SpellElement Element { get; }
    public override (SpellElement, SpellBoostType)[] BoostedElements => [
        (Element, SpellBoostType.Damage),
        (Element, SpellBoostType.Expertise),
        ];

    public override void UpdateEquip(Item item, Player player)
    {
        player.AddElementalDamageBoost(Element, 0.15f);
        player.AddElementalExpertiseBoost(Element, 0.15f);
    }
}

public class DiamondRobeSpellBoost : RobeSpellBoost
{
    public override int ArmorPieceItemID => ItemID.DiamondRobe;
    public override SpellElement Element => SpellElement.Pure;
}

public class RubyRobeSpellBoost : RobeSpellBoost
{
    public override int ArmorPieceItemID => ItemID.RubyRobe;
    public override SpellElement Element => SpellElement.Fire;
}

public class SapphireRobeSpellBoost : RobeSpellBoost
{
    public override int ArmorPieceItemID => ItemID.SapphireRobe;
    public override SpellElement Element => SpellElement.Ice;
}

public class TopazRobeSpellBoost : RobeSpellBoost
{
    public override int ArmorPieceItemID => ItemID.TopazRobe;
    public override SpellElement Element => SpellElement.Holy;
}

public class AmberRobeSpellBoost : RobeSpellBoost
{
    public override int ArmorPieceItemID => ItemID.AmberRobe;
    public override SpellElement Element => SpellElement.Lightning;
}

public class EmeraldRobeSpellBoost : RobeSpellBoost
{
    public override int ArmorPieceItemID => ItemID.EmeraldRobe;
    public override SpellElement Element => SpellElement.Nature;
}

public class AmethystRobeSpellBoost : RobeSpellBoost
{
    public override int ArmorPieceItemID => ItemID.AmethystRobe;
    public override SpellElement Element => SpellElement.Earth;
}

public class MysticRobeSpellBoost : RobeSpellBoost
{
    public override int ArmorPieceItemID => ItemID.GypsyRobe;
    public override SpellElement Element => SpellElement.Dark;
}
