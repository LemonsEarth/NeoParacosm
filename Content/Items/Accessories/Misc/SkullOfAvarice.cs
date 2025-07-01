using NeoParacosm.Common.Utils;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class SkullOfAvarice : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.NPAccessoryPlayer().skullOfAvarice = true;
    }
}
