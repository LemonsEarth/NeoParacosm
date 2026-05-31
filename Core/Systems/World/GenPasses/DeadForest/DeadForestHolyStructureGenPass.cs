using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.DeadForest;

public class DeadForestHolyStructureGenPass : GenPass
{
    public DeadForestHolyStructureGenPass(string name, float loadWeight) : base(name, loadWeight) { }
    readonly string DeadForestHolyStructurePath = "Common/Assets/Structures/DeadForestHolyStructure";
    int baseDeadForestTileRadius = 200;
    int DeadForestRadius => baseDeadForestTileRadius * LemonUtils.GetWorldSize();

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateHolyStructure();
    }

    void GenerateHolyStructure()
    {
        Point16 structureDims = Generator.GetStructureDimensions(DeadForestHolyStructurePath, NeoParacosm.Instance);
        int startXTile = (int)MathHelper.Clamp(Main.dungeonX - DeadForestRadius, 0, Main.maxTilesX);
        int maxXTile = (int)MathHelper.Clamp(Main.dungeonX + DeadForestRadius, 0, Main.maxTilesX);
        int startYTile = (int)MathHelper.Clamp(Main.dungeonY - 80, 0, Main.maxTilesY);
        int maxYTile = (int)MathHelper.Clamp(Main.dungeonY + 80, 0, Main.maxTilesY);

        int maxAttemptCount = 10000;
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
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);

            Point pointAboveTopLeft = new Point(randX, randY - 1);
            Point pointAboveTopRight = new Point(randX + structureDims.X, randY - 1);

            if (Main.tile[pointAboveTopLeft].HasTile && Main.tile[pointAboveTopRight].HasTile)
            {
                attemptCount++;
                continue;
            }

            if (Main.tile[pointTopLeft].HasTile && Main.tile[pointTopRight].HasTile)
            {
                attemptCount++;
                continue;
            }

            if (!LemonUtils.IsTileDeadDirt(pointBottomLeft) || !LemonUtils.IsTileDeadDirt(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(DeadForestHolyStructurePath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        // Replace random walls with unsafe ones
        for (int j = randY; j < randY + structureDims.Y; j++)
        {
            for (int i = randX; i < randX + structureDims.X; i++)
            {
                if (Main.tile[i, j].WallType == WallID.IronBrick)
                {
                    if (WorldGen.genRand.NextBool(2))
                    {
                        Main.tile[i, j].WallType = WallID.Cave2Unsafe;
                    }
                }

                if (Main.tile[i, j].WallType == WallID.Glass)
                {
                    if (WorldGen.genRand.NextBool(2))
                    {
                        Main.tile[i, j].WallType = WallID.None;
                    }
                }
            }
        }

        List<List<(int, int)>> items =
        [
            [(ItemType<GoldenHeartSpell>(), 1)],
            [(ItemID.DiamondRing, 1), (ItemID.AngelHalo, 1)],
            [(ItemID.GoldenCrate, 1), (ItemID.GoldenBugNet, 1), (ItemID.CrossNecklace, 1)],
            [(ItemID.GoldBar, 20), (ItemID.PlatinumBar, 20)],
            [(ItemID.GoldCoin, 10)],
            [(ItemID.GoldenDelight, 3), (ItemID.Milkshake, 3), (ItemID.ApplePie, 3)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 0.8f, 1.5f);
    }
}
