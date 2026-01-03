namespace NeoParacosm.Content.Items.Placeable.Tiles.Depths;

public class DepthStoneWall : ModWall
{
    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = false;
        DustType = DustID.Obsidian;

        AddMapEntry(new Color(5, 1, 66));
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}

public class DepthStoneWallItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 400;
        //ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<ParastoneWallUnsafeItem>();
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(WallType<DepthStoneWall>());
        Item.width = 32;
        Item.height = 32;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe(4);
        recipe.AddIngredient(ItemType<DepthStoneItem>());
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
