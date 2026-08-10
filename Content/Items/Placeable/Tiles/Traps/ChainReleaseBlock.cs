using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using System.Collections.Generic;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Placeable.Tiles.Traps;

public class ChainReleaseBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.DrawsWalls[Type] = true;
        TileID.Sets.DontDrawTileSliced[Type] = true;
        TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
        TileID.Sets.IsAMechanism[Type] = true;

        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileFrameImportant[Type] = true;

        AddMapEntry(new Color(70, 70, 70));
    }


    public override bool IsTileDangerous(int i, int j, Player player) => false;

    public override bool CreateDust(int i, int j, ref int type)
    {
        type = DustID.Iron;
        return true;
    }

    // PlaceInWorld is needed to facilitate styles and alternates since this tile doesn't use a TileObjectData. Placing left and right based on player direction is usually done in the TileObjectData, but the specifics of that don't work for how we want this tile to work.
    public override void PlaceInWorld(int i, int j, Item item)
    {
        int style = Main.LocalPlayer.HeldItem.placeStyle;
        Tile tile = Main.tile[i, j];
        if (Main.LocalPlayer.direction == 1)
        {
            tile.TileFrameX += 18;
        }
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
        }
    }

    // This progression matches vanilla tiles, you don't have to follow it if you don't want. Some vanilla traps don't have 6 states, only 4. This can be implemented with different logic in Slope. Making 8 directions is also easily done in a similar manner.
    private static int[] frameXCycle = [2, 3, 4, 5, 1, 0];
    // We can use the Slope method to override what happens when this tile is hammered.
    public override bool Slope(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        int nextFrameX = frameXCycle[tile.TileFrameX / 18];
        tile.TileFrameX = (short)(nextFrameX * 18);
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
        }
        return false;
    }

    public const int MAX_CHAIN_DISTANCE = 40;
    public override void HitWire(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        // This logic here corresponds to the orientation of the sprites in the spritesheet, change it if your tile is different in design.
        int horizontalDirection = (tile.TileFrameX == 0) ? -1 : ((tile.TileFrameX == 18) ? 1 : 0);
        int verticalDirection = (tile.TileFrameX < 36) ? 0 : ((tile.TileFrameX < 72) ? -1 : 1);
        // Wiring.CheckMech checks if the wiring cooldown has been reached. Put a longer number here for less frequent projectile spawns. 200 is the dart/flame cooldown. Spear is 90, spiky ball is 300
        if (Wiring.CheckMech(i, j, 30))
        {
            int x = i + horizontalDirection;
            int y = j + verticalDirection;
            Tile tileInFront = Main.tile[x, y];
            int count = 0;
            int chainType = TileType<WornChain>();
            if (tileInFront.HasTile && tileInFront.TileType == chainType)
            {
                while (tileInFront.HasTile && tileInFront.TileType == chainType && count < MAX_CHAIN_DISTANCE)
                {
                    WorldGen.KillTile(x, y);
                    count++;
                    x += horizontalDirection;
                    y += verticalDirection;
                    tileInFront = Main.tile[x, y];
                }
            }
            else
            {
                while (!tileInFront.HasTile && count < MAX_CHAIN_DISTANCE)
                {
                    WorldGen.PlaceTile(x, y, chainType);
                    count++;
                    x += horizontalDirection;
                    y += verticalDirection;
                    tileInFront = Main.tile[x, y];
                }
            }
        }
    }
}

public class ChainReleaseItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<ChainReleaseBlock>());
        Item.width = 16;
        Item.height = 16;
    }
}
