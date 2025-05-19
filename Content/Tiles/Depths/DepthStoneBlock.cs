namespace NeoParacosm.Content.Tiles.Depths;

public class DepthStoneBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;

        DustType = DustID.Obsidian;

        AddMapEntry(new Color(13, 2, 99));
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}
