using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Surface;

public class SmallGoblinCampsGenPass : GenPass
{
    public SmallGoblinCampsGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string SmallGoblinCampsPath = "Common/Assets/Structures/SmallGoblinCamps";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < LemonUtils.GetWorldSize(); i++)
        {
            GenerateSmallGoblinCamps(0);
            GenerateSmallGoblinCamps(1);
            GenerateSmallGoblinCamps(2);
        }
    }

    public void GenerateSmallGoblinCamps(int index)
    {
        Point16 structureDims = MultiStructureGenerator.GetStructureDimensions(SmallGoblinCampsPath, NeoParacosm.Instance, index);
        int startXTile = 250;
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

            if (randX > Main.maxTilesX * 0.45f && randX < Main.maxTilesX * 0.55f) // Dont generate in center 10th of the world
            {
                attemptCount++;
                continue;
            }

            randY = Main.rand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottom = new Point(randX + structureDims.X / 2, randY + structureDims.Y);
            Point pointLeft = new Point(randX, randY + structureDims.Y / 2);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            Point pointRight = new Point(randX + structureDims.X, randY + structureDims.Y / 2);

            Point pointSlightlyBelowTop = new Point(randX + structureDims.X / 2, randY + structureDims.Y - 5);

            if (pointTop.HasTile() || pointTopLeft.HasTile() || pointTopRight.HasTile())
            {
                attemptCount++;
                continue;
            }

            if (!pointBottomLeft.IsTileDirtOrGrass() || !pointBottom.IsTileDirtOrGrass() || !pointBottomRight.IsTileDirtOrGrass() || !pointLeft.IsTileDirtOrGrass() || !pointRight.IsTileDirtOrGrass())
            {
                attemptCount++;
                continue;
            }

            MultiStructureGenerator.GenerateMultistructureSpecific(SmallGoblinCampsPath, index, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemID.ClimbingClaws, 1), (ItemID.ShoeSpikes, 1)],
            [(ItemID.AncientIronHelmet, 1), (ItemID.AncientGoldHelmet, 1)],
            [(ItemID.IronChainmail, 1), (ItemID.GoldChainmail, 1), (ItemID.IronBar, 8), (ItemID.IronAxe, 1), (ItemID.IronBroadsword, 1), (ItemID.SpikyBall, 40), (ItemID.PoisonedKnife, 60)],
            [(ItemID.IronGreaves, 1), (ItemID.GoldGreaves, 1), (ItemID.IronBar, 8), (ItemID.GravediggerShovel, 1), (ItemID.IronPickaxe, 1), (ItemID.SpikyBall, 40), (ItemID.PoisonedKnife, 60)],
            [(ItemID.SpikyBall, 40), (ItemID.PoisonedKnife, 80)],
            [(ItemID.GoldCoin, 2), (ItemID.GoldCoin, 3)],
            [(ItemID.TatteredCloth, 4)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 1f, 2f);
    }
}