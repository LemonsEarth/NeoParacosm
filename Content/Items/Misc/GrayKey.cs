using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Misc;

public class GrayKey : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 40;
        Item.value = Item.sellPrice(0, 0, 0, 0);
        Item.rare = ItemRarityID.Yellow;
    }
}
