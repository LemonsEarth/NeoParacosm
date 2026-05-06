using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.DeadForest;

public class DeadForestHolyCrossesGenPass : GenPass
{
    public DeadForestHolyCrossesGenPass(string name, float loadWeight) : base(name, loadWeight) { }
    int baseDeadForestTileRadius = 200;
    int DeadForestRadius => baseDeadForestTileRadius * LemonUtils.GetWorldSize();

    bool IsTileDeadDirt(Point point)
    {
        return Main.tile[point].HasTile && Main.tile[point].TileType == TileType<DeadDirtBlock>();
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateHolyCrosses();
    }

    void GenerateHolyCrosses()
    {
        int attemptCount = 0;
        int maxAttemptCount = 50000;
        int placedCrosses = 0;
        int maxPlacedCrosses = 30 * LemonUtils.GetWorldSize();
        List<int> placedCrossXPos = new List<int>();

        while (attemptCount < maxAttemptCount && placedCrosses < maxPlacedCrosses)
        {
            int startXTile = (int)MathHelper.Clamp(Main.dungeonX - DeadForestRadius, 0, Main.maxTilesX);
            int maxXTile = (int)MathHelper.Clamp(Main.dungeonX + DeadForestRadius, 0, Main.maxTilesX);
            int startYTile = (int)MathHelper.Clamp(Main.dungeonY - 80, 0, Main.maxTilesY);
            int maxYTile = (int)MathHelper.Clamp(Main.dungeonY + 80, 0, Main.maxTilesY);

            Point point = new Point(Main.rand.Next(startXTile, maxXTile), Main.rand.Next(startYTile, maxYTile));
            Point pointBelow = new Point(point.X, point.Y + 1);

            Tile tile = Main.tile[point];
            Tile tileBelow = Main.tile[pointBelow];

            bool skip = false;

            if (!tile.HasTile && tileBelow.HasTile && tileBelow.TileType == TileType<DeadDirtBlock>())
            {
                foreach (var placedCrossXCoord in placedCrossXPos)
                {
                    if (MathF.Abs(placedCrossXCoord - point.X) <= 3)
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    attemptCount++;
                    continue;
                }
                WorldGen.PlaceTile(point.X, point.Y, TileType<HolyCross>());
                placedCrosses++;
                placedCrossXPos.Add(point.X);
            }

            attemptCount++;
        }
        //Main.NewText(placedCrosses);
    }
}
