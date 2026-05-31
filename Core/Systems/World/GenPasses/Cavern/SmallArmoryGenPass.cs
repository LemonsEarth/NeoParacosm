using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.IO;
using Terraria.WorldBuilding;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NeoParacosm.Core.Systems.World.GenPasses.Cavern;

public class SmallArmoryGenPass : GenPass
{
    public SmallArmoryGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string SmallArmoryPath = "Common/Assets/Structures/SmallArmory";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < 8 * LemonUtils.GetWorldSize(); i++)
        {
            GenerateSmallArmory();
        }
    }

    int IsTileValid(Point point)
    {
        if (point.IsTileStone())
        {
            return 1;
        }

        if (point.IsTileSnowy())
        {
            return 2;
        }

        if (point.IsTileSandy())
        {
            return 3;
        }

        return -1;
    }

    public void GenerateSmallArmory()
    {
        Point16 structureDims = Generator.GetStructureDimensions(SmallArmoryPath, NeoParacosm.Instance);
        int startXTile = 200;
        int maxXTile = Main.maxTilesX - 200;
        int startYTile = (int)(Main.rockLayer + 100);
        int maxYTile = Main.maxTilesY - 200;

        int maxAttemptCount = 1000000;
        int attemptCount = 0;
        int randX = 0;
        int randY = 0;
        int isTileValid = -1;
        while (attemptCount < maxAttemptCount)
        {
            randX = WorldGen.genRand.Next(startXTile, maxXTile);
            randY = WorldGen.genRand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);

            isTileValid = IsTileValid(new Point(randX, randY));

            if (isTileValid == -1)
            {
                attemptCount++;
                continue;
            }

            int wallPoints = 0;
            if (pointTopLeft.HasTile())
            {
                wallPoints++;
            }

            if (pointTopRight.HasTile())
            {
                wallPoints++;
            }

            if (pointBottomLeft.HasTile())
            {
                wallPoints++;
            }

            if (pointBottomRight.HasTile())
            {
                wallPoints++;
            }

            if (wallPoints > 3)
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(SmallArmoryPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        bool finishedDoll = false;
        // Replace snow with unsafe walls
        for (int j = randY; j < randY + structureDims.Y; j++)
        {
            for (int i = randX; i < randX + structureDims.X; i++)
            {
                if (Main.tile[i, j].WallType == WallID.GrayBrick)
                {
                    if (isTileValid == 2)
                    {
                        Main.tile[i, j].WallType = WallID.IceBrick;
                    }
                    else if (isTileValid == 3)
                    {
                        Main.tile[i, j].WallType = WallID.SandstoneBrick;
                    }
                }

                if (Main.tile[i, j].TileType == TileID.GrayBrick)
                {
                    if (isTileValid == 2)
                    {
                        Main.tile[i, j].TileType = TileID.IceBrick;
                    }
                    else if (isTileValid == 3)
                    {
                        Main.tile[i, j].TileType = TileID.SandstoneBrick;
                    }
                }

                if (!finishedDoll && Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.DisplayDoll)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity) && tileEntity is TEDisplayDoll doll)
                    {
                        Item[] dollInventory = WorldGenSystem.GetDisplayDollInventory(doll);
                        dollInventory[0] = new(GetRandomHelmet());
                        dollInventory[1] = new(GetRandomChestplate());
                        dollInventory[2] = new(GetRandomGreaves());
                        dollInventory[3] = new(GetRandomAccessory());
                        WorldGenSystem.SetDisplayDollInventory(doll, dollInventory);
                        finishedDoll = true;
                    }
                }

            }
        }
    }

    int GetRandomHelmet()
    {
        return WorldGen.genRand.NextFromList(ItemID.TinHelmet, ItemID.CopperHelmet, ItemID.LeadHelmet, ItemID.IronHelmet, ItemID.GoldHelmet, ItemID.PlatinumHelmet,
            ItemID.SilverHelmet, ItemID.TungstenHelmet, ItemID.GladiatorHelmet, ItemID.AncientCobaltHelmet);
    }

    int GetRandomChestplate()
    {
        return WorldGen.genRand.NextFromList(ItemID.TinChainmail, ItemID.CopperChainmail, ItemID.LeadChainmail, ItemID.IronChainmail, ItemID.GoldChainmail, ItemID.PlatinumChainmail,
            ItemID.SilverChainmail, ItemID.TungstenChainmail, ItemID.GladiatorBreastplate, ItemID.AncientCobaltBreastplate);
    }

    int GetRandomGreaves()
    {
        return WorldGen.genRand.NextFromList(ItemID.TinGreaves, ItemID.CopperGreaves, ItemID.LeadGreaves, ItemID.IronGreaves, ItemID.GoldGreaves, ItemID.PlatinumGreaves,
            ItemID.SilverGreaves, ItemID.TungstenGreaves, ItemID.GladiatorLeggings, ItemID.AncientCobaltLeggings);
    }

    int GetRandomAccessory()
    {
        return WorldGen.genRand.NextFromList(ItemID.Aglet, ItemID.ShoeSpikes, ItemID.ClimbingClaws, ItemID.LuckyHorseshoe,
            ItemID.IceSkates, ItemID.BandofRegeneration, ItemID.Shackle, ItemID.SquireShield);
    }
}