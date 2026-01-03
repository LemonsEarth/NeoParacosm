namespace NeoParacosm.Content.Items.Placeable.Special.ResearcherQuestTiles;

public class DragonRemainsPlacer : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Furnace;
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<DragonRemainsTile>());
        Item.width = 40;
        Item.height = 40;
        Item.rare = ItemRarityID.Expert;
    }
}
