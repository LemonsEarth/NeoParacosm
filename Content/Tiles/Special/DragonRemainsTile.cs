using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;

namespace NeoParacosm.Content.Tiles.Special;

public class DragonRemainsTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.UsesCustomCanPlace = true;
        //TileObjectData.newTile.StyleHorizontal = true;
        //TileObjectData.newTile.StyleWrapLimit = 15;
        TileObjectData.newTile.Height = 10;
        TileObjectData.newTile.Width = 15;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidBottom, 15, 0);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(5, 1, 66));
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}
