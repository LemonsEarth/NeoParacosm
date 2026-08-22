using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using System.Collections.ObjectModel;

namespace NeoParacosm.Core.UI.Spells;

public class SpellIcon : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 0);
        Item.rare = ItemRarityID.White;
    }

    public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
    {
        return true;
    }
}