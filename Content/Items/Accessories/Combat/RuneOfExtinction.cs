using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class RuneOfExtinction : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 15, false));
        ItemID.Sets.AnimatesAsSoul[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 54;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 3, 0, 0);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.NPAccessoryPlayer().runeOfExtinction = true;
    }
}
