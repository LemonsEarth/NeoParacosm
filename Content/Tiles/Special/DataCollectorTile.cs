using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Tiles.Special;

public class DataCollectorTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;

        MinPick = 50;
        AnimationFrameHeight = 90;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.UsesCustomCanPlace = true;
        //TileObjectData.newTile.StyleHorizontal = true;
        //TileObjectData.newTile.StyleWrapLimit = 15;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
        TileObjectData.newTile.Height = 5;
        TileObjectData.newTile.Width = 5;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };

        TileObjectData.newTile.CoordinatePadding = 2;
        //TileObjectData.newTile.Origin = new Point16(0, 5);
        //TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 5, 0);
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<DataCollectorTileEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(255, 50, 50));
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.9f;
        g = 0.9f;
        b = 0.9f;
    }

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        frameCounter++;
        if (frameCounter > 12)
        {
            frameCounter = 0;
            frame++;
        }
        if (frame > 3)
        {
            frame = 0;
        }
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        ModContent.GetInstance<DataCollectorTileEntity>().Kill(i, j);
    }
}
