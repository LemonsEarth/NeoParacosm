using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.DeadForest;

public class DeadForestBasementGenPass : GenPass
{
    public DeadForestBasementGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string DeadForestBasementPath = "Common/Assets/Structures/DeadForestBasement";
    int baseDeadForestTileRadius = 200;
    int DeadForestRadius => baseDeadForestTileRadius * LemonUtils.GetWorldSize();

    bool IsTileDeadDirt(Point point)
    {
        return Main.tile[point].HasTile && Main.tile[point].TileType == TileType<DeadDirtBlock>();
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateBasement();
    }

    void GenerateBasement()
    {
        Point16 structureDims = Generator.GetStructureDimensions(DeadForestBasementPath, NeoParacosm.Instance);

        int startXTile = (int)MathHelper.Clamp(Main.dungeonX - DeadForestRadius, 0, Main.maxTilesX);
        int maxXTile = (int)MathHelper.Clamp(Main.dungeonX + DeadForestRadius, 0, Main.maxTilesX);
        int startYTile = (int)MathHelper.Clamp(Main.dungeonY - 80, 0, Main.maxTilesY);
        int maxYTile = (int)MathHelper.Clamp(Main.dungeonY + 80, 0, Main.maxTilesY);

        int maxAttemptCount = 10000;
        int attemptCount = 0;
        while (attemptCount < maxAttemptCount)
        {
            int randX = Main.rand.Next(startXTile, maxXTile);
            int randY = Main.rand.Next(startYTile, maxYTile);
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

            if (!IsTileDeadDirt(pointTopLeft) || !IsTileDeadDirt(pointTopRight) || !IsTileDeadDirt(pointBottomLeft) || !IsTileDeadDirt(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(DeadForestBasementPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }
    }
}
