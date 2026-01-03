namespace NeoParacosm.Content.Items.Placeable.Tiles.Depths;

public class DepthStoneBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        HitSound = SoundID.Tink;

        DustType = DustID.Obsidian;

        AddMapEntry(new Color(13, 2, 99));
    }

    public override void RandomUpdate(int i, int j)
    {
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                if (WorldGen.InWorld(i + x, j + y) && Main.tile[i + x, j + y].LiquidAmount == 0 && !Main.tile[i + x, j + y].HasTile)
                {
                    Tile tile = Main.tile[i + x, j + y];
                    tile.LiquidType = LiquidID.Water;
                    tile.LiquidAmount = 255;
                }
            }
        }
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}

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
