namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class DeadDirtWall : ModWall
{
    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = false;
        DustType = DustID.Ash;

        AddMapEntry(new Color(105, 99, 94));
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}

public class DeadDirtWallItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 400;
        //ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<ParastoneWallUnsafeItem>();
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(WallType<DeadDirtWall>());
        Item.width = 32;
        Item.height = 32;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe(4);
        recipe.AddIngredient(ItemType<DeadDirtItem>());
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}
