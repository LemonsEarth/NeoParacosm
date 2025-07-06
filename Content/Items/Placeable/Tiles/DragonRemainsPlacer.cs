using NeoParacosm.Content.Tiles.Special;

namespace NeoParacosm.Content.Items.Placeable.Tiles;

public class DragonRemainsPlacer : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Furnace;
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<DragonRemainsTile>());
        Item.width = 40;
        Item.height = 40;
        Item.rare = ItemRarityID.Expert;
    }
}
