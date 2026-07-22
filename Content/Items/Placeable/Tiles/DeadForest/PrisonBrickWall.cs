namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class PrisonBrickWall : ModWall
{
    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = false;
        DustType = DustID.Ash;

        AddMapEntry(new Color(75, 69, 64));
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}

public class PrisonBrickWallItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 400;
        //ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<ParastoneWallUnsafeItem>();
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(WallType<PrisonBrickWall>());
        Item.width = 32;
        Item.height = 32;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe(4);
        recipe.AddIngredient(ItemType<PrisonBrickItem>());
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
