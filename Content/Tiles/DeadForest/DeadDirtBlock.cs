namespace NeoParacosm.Content.Tiles.DeadForest;

public class DeadDirtBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        //Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        HitSound = SoundID.Dig;

        DustType = DustID.Dirt;
        Main.tileMerge[TileID.Stone][Type] = true;
        Main.tileMerge[TileID.Sand][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        TileID.Sets.ChecksForMerge[Type] = true;
        AddMapEntry(new Color(105, 99, 94));
    }

    public override void RandomUpdate(int i, int j)
    {

    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}
