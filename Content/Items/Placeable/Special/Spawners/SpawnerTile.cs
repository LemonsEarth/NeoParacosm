using NeoParacosm.Content.Items.Placeable.Machines;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Special.Spawners;

public abstract class SpawnerTile : ModTile
{
    /// <summary>
    /// Should be GetInstance<YourSpawnerTileEntity>()
    /// </summary>
    public abstract SpawnerTileEntity TileEntityType { get; }

    public override void SetStaticDefaults()
    {
        //TileID.Sets.DrawsWalls[Type] = true;
        //TileID.Sets.DontDrawTileSliced[Type] = true;
        TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
        DustType = DustID.Stone;
        MineResist = 8f;
        MinPick = 55;

        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;
        Main.tileFrameImportant[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Width = 2;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.Height = 2;
        TileObjectData.newTile.CoordinateHeights = [16, 16];
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 2, 0);
        TileObjectData.newTile.HookPostPlaceMyPlayer = TileEntityType.Generic_HookPostPlaceMyPlayer;
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(129, 0, 0));
    }


    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        TileEntityType.Kill(i, j);
    }
}

public abstract class SpawnerTileItem : ModItem
{
    public abstract int TileType { get; }
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType);
        Item.width = 16;
        Item.height = 16;
    }
}
