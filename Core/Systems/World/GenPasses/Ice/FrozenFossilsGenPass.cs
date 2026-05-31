using StructureHelper.API;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Ice;

public class FrozenFossilsGenPass : GenPass
{
    public FrozenFossilsGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string FrozenFossilSkull = "Common/Assets/Structures/FrozenFossilSkull";
    readonly string FrozenFossilBone1 = "Common/Assets/Structures/FrozenFossilBone1";
    readonly string FrozenFossilBone2 = "Common/Assets/Structures/FrozenFossilBone2";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < LemonUtils.GetWorldSize(); i++)
        {
            GenerateFossilSkull();
            GenerateFossilBone1();
            GenerateFossilBone2();
        }
    }

    public void GenerateFossilSkull()
    {
        Point16 structureDims = Generator.GetStructureDimensions(FrozenFossilSkull, NeoParacosm.Instance);
        int startXTile = GenVars.snowOriginLeft;
        int maxXTile = GenVars.snowOriginRight;
        int startYTile = (int)(Main.rockLayer);
        int maxYTile = GenVars.snowBottom;

        int maxAttemptCount = 1000000;
        int attemptCount = 0;
        int randX = 0;
        int randY = 0;
        while (attemptCount < maxAttemptCount)
        {
            randX = WorldGen.genRand.Next(startXTile, maxXTile);
            randY = WorldGen.genRand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            int snowPoints = 0;

            if (LemonUtils.IsTileSnowy(pointTopRight))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointTopRight))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointBottomLeft))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointBottomRight))
            {
                snowPoints++;
            }

            // exactly 2 sides must be in ice/snow and 2 exposed
            if (snowPoints != 2)
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(FrozenFossilSkull, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }
    }

    public void GenerateFossilBone1()
    {
        Point16 structureDims = Generator.GetStructureDimensions(FrozenFossilBone1, NeoParacosm.Instance);
        int startXTile = GenVars.snowOriginLeft;
        int maxXTile = GenVars.snowOriginRight;
        int startYTile = (int)(Main.rockLayer);
        int maxYTile = GenVars.snowBottom;

        int maxAttemptCount = 1000000;
        int attemptCount = 0;
        int randX = 0;
        int randY = 0;
        while (attemptCount < maxAttemptCount)
        {
            randX = WorldGen.genRand.Next(startXTile, maxXTile);
            randY = WorldGen.genRand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            int snowPoints = 0;

            if (LemonUtils.IsTileSnowy(pointTopRight))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointTopRight))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointBottomLeft))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointBottomRight))
            {
                snowPoints++;
            }

            // exactly 2 sides must be in ice/snow and 2 exposed
            if (snowPoints != 2)
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(FrozenFossilBone1, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }
    }

    public void GenerateFossilBone2()
    {
        Point16 structureDims = Generator.GetStructureDimensions(FrozenFossilBone2, NeoParacosm.Instance);
        int startXTile = GenVars.snowOriginLeft;
        int maxXTile = GenVars.snowOriginRight;
        int startYTile = (int)(Main.rockLayer);
        int maxYTile = GenVars.snowBottom;

        int maxAttemptCount = 1000000;
        int attemptCount = 0;
        int randX = 0;
        int randY = 0;
        while (attemptCount < maxAttemptCount)
        {
            randX = WorldGen.genRand.Next(startXTile, maxXTile);
            randY = WorldGen.genRand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            int snowPoints = 0;

            if (LemonUtils.IsTileSnowy(pointTopRight))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointTopRight))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointBottomLeft))
            {
                snowPoints++;
            }

            if (LemonUtils.IsTileSnowy(pointBottomRight))
            {
                snowPoints++;
            }

            // exactly 2 sides must be in ice/snow and 2 exposed
            if (snowPoints != 2)
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(FrozenFossilBone2, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }
    }
}