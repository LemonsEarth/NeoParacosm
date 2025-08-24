using NeoParacosm.Content.Tiles.Depths;

namespace NeoParacosm.Content.Items.Placeable.Tiles;

public class DepthStoneItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.StoneBlock;
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<DepthStoneBlock>());
        Item.width = 16;
        Item.height = 16;
    }

    public override void AddRecipes()
    {

    }
}
