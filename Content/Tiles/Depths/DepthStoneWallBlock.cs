namespace NeoParacosm.Content.Tiles.Depths;

public class DepthStoneWallBlock : ModWall
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
