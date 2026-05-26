using NeoParacosm.Content.Items.Accessories.Combat.Ranged;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Surface;

public class GoblinWatchtowerGenPass : GenPass
{
    public GoblinWatchtowerGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string GoblinWatchtowerPath = "Common/Assets/Structures/GoblinWatchtower";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateGoblinWatchtowerLeft();
        GenerateGoblinWatchtowerRight();
    }

    public void GenerateGoblinWatchtowerLeft()
    {
        Point16 structureDims = Generator.GetStructureDimensions(GoblinWatchtowerPath, NeoParacosm.Instance);
        int startXTile = 250;
        int maxXTile = Main.maxTilesX / 5;
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
            Point pointBottom = new Point(randX + structureDims.X / 2, randY + structureDims.Y);
            Point pointLeft = new Point(randX, randY + structureDims.Y / 2);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            Point pointRight = new Point(randX + structureDims.X, randY + structureDims.Y / 2);

            Point pointSlightlyAboveBottom = new Point(randX + structureDims.X / 2, randY + structureDims.Y - 5);

            if (pointTop.HasTile() || pointLeft.HasTile() || pointRight.HasTile())
            {
                attemptCount++;
                continue;
            }

            if (!pointBottomLeft.HasTile() || !pointBottom.HasTile() || !pointBottomRight.HasTile())
            {
                attemptCount++;
                continue;
            }

            if (pointSlightlyAboveBottom.HasTile())
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(GoblinWatchtowerPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemType<SharpenedArrowhead>(), 1), (ItemID.CreativeWings, 1)],
            [(ItemID.Compass, 1), (ItemID.Radar, 1)],
            [(ItemID.GoldBow, 1), (ItemID.PlatinumBow, 1)],
            [(ItemID.Harpoon, 1), (ItemID.SpikyBall, 40)],
            [(ItemID.GoldCoin, 5), (ItemID.GoldCoin, 10)],
            [(ItemID.TatteredCloth, 6)],
            [(ItemID.WoodenArrow, 80)],
            [(ItemID.FlamingArrow, 60), (ItemID.FrostburnArrow, 60)],
            [(ItemID.JestersArrow, 40), (ItemID.UnholyArrow, 40)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 1f, 1.5f);
    }

    public void GenerateGoblinWatchtowerRight()
    {
        Point16 structureDims = Generator.GetStructureDimensions(GoblinWatchtowerPath, NeoParacosm.Instance);
        int startXTile = (int)(Main.maxTilesX * 0.8f);
        int maxXTile = Main.maxTilesX - 250;
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
            Point pointBottom = new Point(randX + structureDims.X / 2, randY + structureDims.Y);
            Point pointLeft = new Point(randX, randY + structureDims.Y / 2);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            Point pointRight = new Point(randX + structureDims.X, randY + structureDims.Y / 2);

            Point pointSlightlyAboveBottom = new Point(randX + structureDims.X / 2, randY + structureDims.Y - 5);

            if (pointTop.HasTile() || pointLeft.HasTile() || pointRight.HasTile())
            {
                attemptCount++;
                continue;
            }

            if (!pointBottomLeft.HasTile() || !pointBottom.HasTile() || !pointBottomRight.HasTile())
            {
                attemptCount++;
                continue;
            }

            if (pointSlightlyAboveBottom.HasTile())
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(GoblinWatchtowerPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemType<SharpenedArrowhead>(), 1), (ItemID.CreativeWings, 1)],
            [(ItemID.Compass, 1), (ItemID.Radar, 1)],
            [(ItemID.GoldBow, 1), (ItemID.PlatinumBow, 1)],
            [(ItemID.Harpoon, 1), (ItemID.SpikyBall, 40)],
            [(ItemID.GoldCoin, 5), (ItemID.GoldCoin, 10)],
            [(ItemID.TatteredCloth, 6)],
            [(ItemID.WoodenArrow, 80)],
            [(ItemID.FlamingArrow, 60), (ItemID.FrostburnArrow, 60)],
            [(ItemID.JestersArrow, 40), (ItemID.UnholyArrow, 40)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 1f, 1.5f);
    }
}