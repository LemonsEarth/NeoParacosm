using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Cavern;

public class UndergroundCheckpointGemsGenPass : GenPass
{
    public UndergroundCheckpointGemsGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string UndergroundCheckpointGemsPath = "Common/Assets/Structures/UndergroundCheckpointGems";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < 6 * LemonUtils.GetWorldSize(); i++)
        {
            GenerateUndergroundCheckpointGems();
        }
    }

    public void GenerateUndergroundCheckpointGems()
    {
        Point16 structureDims = Generator.GetStructureDimensions(UndergroundCheckpointGemsPath, NeoParacosm.Instance);
        int startXTile = 200;
        int maxXTile = Main.maxTilesX - 200;
        int startYTile = (int)(Main.rockLayer);
        int maxYTile = Main.maxTilesY - 400;

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
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBelowBottomLeft = new Point(randX, randY + structureDims.Y + 1);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            Point pointBelowBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y + 1);

            if (!pointBelowBottomLeft.HasTile() || !pointBelowBottomRight.HasTile())
            {
                attemptCount++;
                continue;
            }
            if (pointTopLeft.HasTile() || pointTopRight.HasTile())
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(UndergroundCheckpointGemsPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemID.IronBar, 8), (ItemID.LeadBar, 8), (ItemID.CopperBar, 8), (ItemID.TinBar, 8)],
            [(ItemID.GoldBar, 8), (ItemID.PlatinumBar, 8), (ItemID.SilverBar, 8), (ItemID.TungstenBar, 8)],
            [(ItemID.CrimtaneBar, 8), (ItemID.DemoniteBar, 8), (ItemID.GoldBar, 8), (ItemID.PlatinumBar, 8)],
            [(ItemID.IronOre, 12), (ItemID.LeadOre, 12), (ItemID.CopperOre, 12), (ItemID.TinOre, 12)],
            [(ItemID.GoldOre, 12), (ItemID.PlatinumOre, 12), (ItemID.SilverOre, 12), (ItemID.TungstenOre, 12)],
            [(ItemID.CrimtaneOre, 12), (ItemID.DemoniteOre, 12), (ItemID.GoldOre, 12), (ItemID.PlatinumOre, 12)],
            [(ItemID.SilverPickaxe, 12), (ItemID.TungstenPickaxe, 12)],
            [(ItemID.Bomb, 8), (ItemID.Dynamite, 4)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 1f, 1.5f);
    }
}