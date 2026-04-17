using System.Collections.Generic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Misc;
using Terraria.Localization;

namespace NeoParacosm.Core.Globals.GlobalItems.Armors;

public abstract class SpellBoostArmorPiece : GlobalItem
{
    public abstract int ArmorPieceItemID { get; }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return lateInstantiation && entity.type == ArmorPieceItemID;
    }

    public abstract (SpellElement, SpellBoostType)[] BoostedElements { get; }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!KeybindSystem.InspectItem.Current)
        {
            TooltipLine inspectTooltip = new TooltipLine(Mod, "NeoParacosm:InspectTooltip", Language.GetTextValue("Mods.NeoParacosm.Items.LoreTemplate.Inspect"));
            tooltips.Add(inspectTooltip);
            return;
        }
        foreach ((SpellElement, SpellBoostType) elementBoost in BoostedElements)
        {
            tooltips.Add(LemonUtils.QuickArmorSpellBoostTooltipLine($"Item{ArmorPieceItemID}", elementBoost.Item1, elementBoost.Item2));
        }
    }
}
