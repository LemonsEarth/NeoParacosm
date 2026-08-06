namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class BonestoneBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBrick[Type] = true;
        //Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        HitSound = SoundID.Dig;
        MineResist = 2f;
        DustType = DustID.Dirt;
        TileID.Sets.ChecksForMerge[Type] = true;
        Main.tileMerge[TileType<DeadDirtBlock>()][Type] = true;
        AddMapEntry(new Color(84, 84, 84));
    }

    public override void RandomUpdate(int i, int j)
    {

    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }

    public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
    {
        // We use this method to set the merge values of the adjacent tiles to -2 if the tile nearby is a pearlsandstone block
        // -2 is what terraria uses to designate the tiles that will merge with ours using the custom frames
        WorldGen.TileMergeAttempt(-2, TileType<DeadDirtBlock>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
    }
}

public class BonestoneBlockItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.GrayBrick;
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<BonestoneBlock>());
        Item.width = 16;
        Item.height = 16;
    }

    public override void AddRecipes()
    {
        CreateRecipe(100)
            .AddIngredient(ItemID.StoneBlock, 100)
            .AddIngredient(ItemID.Tombstone, 1)
            .AddCondition(Condition.InGraveyard)
            .Register();
    }
}
