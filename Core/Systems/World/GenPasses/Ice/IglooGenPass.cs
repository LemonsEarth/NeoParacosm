using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;
using NeoParacosm.Content.Items.Weapons.Melee;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Ice;

public class IglooGenPass : GenPass
{
    public IglooGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string IglooPath = "Common/Assets/Structures/Igloo";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateIgloo();
    }

    public void GenerateIgloo()
    {
        Point16 structureDims = Generator.GetStructureDimensions(IglooPath, NeoParacosm.Instance);
        int startXTile = 400;
        int maxXTile = Main.maxTilesX - 400;
        int startYTile = (int)(Main.worldSurface * 0.33f);
        int maxYTile = ((int)Main.worldSurface);

        int maxAttemptCount = 1000000;
        int attemptCount = 0;
        int randX = 0;
        int randY = 0;
        while (attemptCount < maxAttemptCount)
        {
            randX = Main.rand.Next(startXTile, maxXTile);
            randY = Main.rand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);

            if (Main.tile[pointTop].HasTile
                || (Main.tile[pointTopLeft].HasTile && Main.tile[pointTopRight].HasTile)
                || !LemonUtils.IsTileSnowy(pointBottomLeft) || !LemonUtils.IsTileSnowy(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(IglooPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        bool manDone = false;
        bool womanDone = false;
        // Replace snow with unsafe walls
        for (int j = randY; j < randY + structureDims.Y; j++)
        {
            for (int i = randX; i < randX + structureDims.X; i++)
            {
                if (Main.tile[i, j].WallType == WallID.SnowWallEcho)
                {
                    Main.tile[i, j].WallType = WallID.SnowWallUnsafe;
                }

                if (!manDone && Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.DisplayDoll && Main.tile[i,j].TileFrameX == 0)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity) && tileEntity is TEDisplayDoll doll)
                    {
                        Item[] dollInventory = WorldGenSystem.GetDisplayDollInventory(doll);
                        dollInventory[0] = new(ItemID.EskimoHood);
                        dollInventory[1] = new(ItemID.EskimoCoat);
                        dollInventory[2] = new(ItemID.EskimoPants);
                        dollInventory[3] = new(ItemID.BandofStarpower);
                        WorldGenSystem.SetDisplayDollInventory(doll, dollInventory);
                        manDone = true;
                    }
                }

                if (!womanDone && Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.DisplayDoll && Main.tile[i, j].TileFrameX == 72)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity) && tileEntity is TEDisplayDoll doll)
                    {
                        Item[] dollInventory = WorldGenSystem.GetDisplayDollInventory(doll);
                        dollInventory[0] = new(ItemID.PinkEskimoHood);
                        dollInventory[1] = new(ItemID.PinkEskimoCoat);
                        dollInventory[2] = new(ItemID.PinkEskimoPants);
                        dollInventory[3] = new(ItemID.BandofRegeneration);
                        WorldGenSystem.SetDisplayDollInventory(doll, dollInventory);
                        womanDone = true;
                    }
                }
            }
        }

        List<List<(int, int)>> items =
        [
            [(ItemType<SnowgraveSpell>(), 1)],
            [(ItemID.SnowballLauncher, 1), (ItemID.IceSkates, 1)],
            [(ItemID.WarmthPotion, 3)],
            [(ItemID.Snowball, 40)],
            [(ItemID.BorealWood, 40)],
            [(ItemID.SnowBlock, 25)],
            [(ItemID.IceBlock, 25)]
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 1f, 2f);
    }
}