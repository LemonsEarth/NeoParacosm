namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class DeadDirtBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        //Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        HitSound = SoundID.Dig;
        TileID.Sets.Grass[Type] = true;
        TileID.Sets.Conversion.Grass[Type] = true;
        DustType = DustID.Dirt;
        Main.tileMerge[TileID.Stone][Type] = true;
        Main.tileMerge[TileID.Sand][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        //Main.tileMerge[TileType<BonestoneBlock>()][Type] = true;
        TileID.Sets.ChecksForMerge[Type] = true;
        AddMapEntry(new Color(77, 71, 71));
    }

    public override void RandomUpdate(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        Tile tileAbove = Main.tile[i, j - 1];
        if (WorldGen.genRand.NextBool(10) && !tileAbove.HasTile && tileAbove.LiquidAmount == 0 && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
        {
            tileAbove.TileType = (ushort)TileType<DeadShortPlants>();
            tileAbove.HasTile = true;
            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(0, 8) * 18);
            tileAbove.TileFrameY = 0;

            WorldGen.SquareTileFrame(i, j - 1, true);

            if (Main.dedServ)
            {
                NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
            }
        }
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}

public class DeadDirtItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.DirtBlock;
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<DeadDirtBlock>());
        Item.width = 16;
        Item.height = 16;
    }

    public override void AddRecipes()
    {
        CreateRecipe(100)
            .AddIngredient(ItemID.DirtBlock, 100)
            .AddIngredient(ItemID.Tombstone, 1)
            .AddCondition(Condition.InGraveyard)
            .Register();
    }
}
