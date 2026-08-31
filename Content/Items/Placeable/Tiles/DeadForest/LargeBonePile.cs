using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class LargeBonePile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;

        TileID.Sets.GeneralPlacementTiles[Type] = false;
        TileID.Sets.PreventsSandfall[Type] = true;

        TileObjectData.newTile.UsesCustomCanPlace = true;
        //TileObjectData.newTile.StyleHorizontal = true;
        //TileObjectData.newTile.StyleWrapLimit = 15;
        TileObjectData.newTile.Height = 4;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.Width = 6;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 6, 0);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(100, 100, 100));
    }

    public override bool CanDrop(int i, int j)
    {
        return false;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}

public class LargeBonePileItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<LargeBonePile>());
        Item.width = 96;
        Item.height = 54;
    }
}
