using NeoParacosm.Core.Systems.Misc;
using System.Collections.Generic;
using Terraria.Localization;

namespace NeoParacosm.Core.Globals.GlobalItems;

public class NPGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;


    public override void SetStaticDefaults()
    {
        
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.ModItem == null) return;
        if (Language.Exists($"Mods.NeoParacosm.Items.{item.ModItem.Name}.NPLore"))
        {
            if (KeybindSystem.InspectItem.Current)
            {
                TooltipLine loreTooltip = new TooltipLine(Mod, "NeoParacosm:InspectLore", Language.GetTextValue($"Mods.NeoParacosm.Items.{item.ModItem.Name}.NPLore"));
                tooltips.Add(loreTooltip);
            }
            else
            {
                TooltipLine inspectTooltip = new TooltipLine(Mod, "NeoParacosm:InspectTooltip", Language.GetTextValue("Mods.NeoParacosm.Items.LoreTemplate.Inspect"));
                tooltips.Add(inspectTooltip);
            }
        }
    }
}
