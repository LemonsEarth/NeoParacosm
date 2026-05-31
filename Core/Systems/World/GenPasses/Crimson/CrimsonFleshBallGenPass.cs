using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Crimson;

public class CrimsonFleshBallGenPass : GenPass
{
    public CrimsonFleshBallGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string CrimsonFleshBallPath = "Common/Assets/Structures/CrimsonFleshBall";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        if (!WorldGen.crimson)
        {
            return;
        }

        GenerateCrimsonFleshBall();
    }

    void GenerateWithLoot(int tileX, int tileY, Point16 structureDims)
    {
        Point pointTopLeft = new Point(tileX, tileY);
        Generator.GenerateStructure(CrimsonFleshBallPath, new Point16(pointTopLeft), NeoParacosm.Instance);

        List<List<(int, int)>> items =
        [
            [(ItemID.TheUndertaker, 1), (ItemID.TheRottedFork, 1), (ItemID.CrimsonRod, 1)],
            [(ItemID.CrimsonHeart, 1), (ItemID.PanicNecklace, 1)],
            [(ItemID.TheUndertaker, 1), (ItemID.CrimsonHeart, 1), (ItemID.TheRottedFork, 1), (ItemID.PanicNecklace, 1), (ItemID.CrimsonRod, 1)],
        ];

        LemonUtils.GenerateStructureLoot(tileX, tileY, structureDims, items, 0.8f, 1.5f);
    }

    public void GenerateCrimsonFleshBall()
    {
        Point16 structureDims = Generator.GetStructureDimensions(CrimsonFleshBallPath, NeoParacosm.Instance);
        int startXTile = 200;
        int maxXTile = Main.maxTilesX - 200;
        int startYTile = (int)(Main.worldSurface * 0.33f);
        int maxYTile = ((int)Main.worldSurface);

        int maxAttemptCount = 1000000;
        int attemptCount = 0;
        while (attemptCount < maxAttemptCount)
        {
            int randX = WorldGen.genRand.Next(startXTile, maxXTile);
            int randY = WorldGen.genRand.Next(startYTile, maxYTile);

            Point pointTopLeft = new Point(randX, randY);
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointRight = new Point(randX + structureDims.X, randY + structureDims.Y / 2);
            Point pointLeft = new Point(randX, randY + structureDims.Y / 2);
            Point pointBottom = new Point(randX + structureDims.X / 2, randY + structureDims.Y);

            // One side should always be outside blocks, while at least 2 are inside blocks

            if (LemonUtils.IsTileCrimson(pointTop)
                && (LemonUtils.IsTileCrimson(pointLeft) || LemonUtils.IsTileCrimson(pointRight))
                && !LemonUtils.IsTileCrimson(pointBottom))
            {
                GenerateWithLoot(randX, randY, structureDims);
                break;
            }

            if (LemonUtils.IsTileCrimson(pointBottom)
                && (LemonUtils.IsTileCrimson(pointLeft) || LemonUtils.IsTileCrimson(pointRight))
                && !LemonUtils.IsTileCrimson(pointTop))
            {
                GenerateWithLoot(randX, randY, structureDims);
                break;
            }

            attemptCount++;
        }
    }
}