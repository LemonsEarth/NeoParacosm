using StructureHelper.API;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Cavern;

public class LargeArmoryGenPass : GenPass
{
    public LargeArmoryGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string LargeArmoryPath = "Common/Assets/Structures/LargeArmory";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < 3 * LemonUtils.GetWorldSize(); i++)
        {
            GenerateLargeArmory();
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

    public void GenerateLargeArmory()
    {
        Point16 structureDims = Generator.GetStructureDimensions(LargeArmoryPath, NeoParacosm.Instance);
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

            int points = 0;

            if (pointTopLeft.HasTile())
            {
                points++;
            }

            if (pointTopRight.HasTile())
            {
                points++;
            }

            if (pointBottomLeft.HasTile())
            {
                points++;
            }

            if (pointBottomRight.HasTile())
            {
                points++;
            }

            if (points > 2)
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(LargeArmoryPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

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

                if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.DisplayDoll)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity) && tileEntity is TEDisplayDoll doll)
                    {
                        Item[] dollInventory = WorldGenSystem.GetDisplayDollInventory(doll);
                        dollInventory[0] = new(GetRandomHelmet());
                        dollInventory[1] = new(GetRandomChestplate());
                        dollInventory[2] = new(GetRandomGreaves());
                        dollInventory[3] = new(GetRandomAccessory());
                        dollInventory[4] = new(GetRandomAccessory());
                        WorldGenSystem.SetDisplayDollInventory(doll, dollInventory);
                    }
                }

            }
        }
    }

    int GetRandomHelmet()
    {
        if (Main.rand.NextBool(4))
        {
            return ItemID.None;
        }
        return WorldGen.genRand.NextFromList(ItemID.AncientIronHelmet, ItemID.NinjaHood,
            ItemID.AnglerHat, ItemID.AncientShadowHelmet, ItemID.GladiatorHelmet, ItemID.AncientCobaltHelmet);
    }

    int GetRandomChestplate()
    {
        if (Main.rand.NextBool(4))
        {
            return ItemID.None;
        }
        return WorldGen.genRand.NextFromList(ItemID.IronChainmail, ItemID.NinjaShirt,
            ItemID.AnglerVest, ItemID.AncientShadowScalemail, ItemID.GladiatorBreastplate, ItemID.AncientCobaltBreastplate);
    }

    int GetRandomGreaves()
    {
        if (Main.rand.NextBool(4))
        {
            return ItemID.None;
        }
        return WorldGen.genRand.NextFromList(ItemID.IronGreaves, ItemID.NinjaPants,
            ItemID.AnglerPants, ItemID.AncientShadowGreaves, ItemID.GladiatorLeggings, ItemID.AncientCobaltLeggings);
    }

    int GetRandomAccessory()
    {
        if (Main.rand.NextBool(2))
        {
            return ItemID.None;
        }
        return WorldGen.genRand.NextFromList(ItemID.CrossNecklace, ItemID.LuckyHorseshoe,
            ItemID.FeralClaws, ItemID.PhilosophersStone, ItemID.SquireShield, ItemID.AncientChisel, ItemID.HermesBoots, ItemID.CelestialMagnet);
    }
}