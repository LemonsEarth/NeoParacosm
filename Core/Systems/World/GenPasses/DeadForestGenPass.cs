using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses
{
    public class DeadForestGenPass : GenPass
    {
        public DeadForestGenPass(string name, float loadWeight) : base(name, loadWeight) { }

        readonly string DeadForestPlatformsPath = "Common/Assets/Structures/DeadForestPlatforms";
        readonly string DeadForestBasementPath = "Common/Assets/Structures/DeadForestBasement";
        readonly string DeadForestHolyStructurePath = "Common/Assets/Structures/DeadForestHolyStructure";
        int baseDeadForestTileRadius = 200;
        int DeadForestRadius => baseDeadForestTileRadius * LemonUtils.GetWorldSize();

        bool IsTileDeadDirt(Point point)
        {
            return Main.tile[point].HasTile && Main.tile[point].TileType == TileType<DeadDirtBlock>();
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            Point startPos = new Point(Main.dungeonX, Main.dungeonY + 30);
            for (int i = -DeadForestRadius; i < DeadForestRadius; i++)
            {
                for (int j = -DeadForestRadius; j < DeadForestRadius; j++)
                {
                    Point pos = startPos + new Point(i, j);
                    pos = new Point(Math.Clamp(pos.X, 0, Main.maxTilesX), Math.Clamp(pos.Y, 0, Main.maxTilesY));
                    if (startPos.ToWorldCoordinates().Distance(pos.ToWorldCoordinates()) > DeadForestRadius * 16)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[pos];
                    if (tile.HasTile)
                    {
                        switch (tile.TileType)
                        {
                            case TileID.Dirt or TileID.ClayBlock or TileID.Grass or TileID.Sand or TileID.CorruptGrass or TileID.CrimsonGrass or TileID.Ebonsand or TileID.Crimsand:
                                WorldGen.ConvertTile(pos.X, pos.Y, TileType<DeadDirtBlock>());
                                break;
                        }
                    }
                }
            }

            GenerateDeadForestPlatforms();
            GenerateBasement();
            GenerateHolyStructure();
            GenerateHolyCrosses();
        }

        void GenerateDeadForestPlatforms()
        {
            int startXTile = (int)MathHelper.Clamp(Main.dungeonX - DeadForestRadius, 0, Main.maxTilesX);
            int maxXTile = (int)MathHelper.Clamp(Main.dungeonX + DeadForestRadius, 0, Main.maxTilesX);
            int startYTile = (int)MathHelper.Clamp(Main.dungeonY - 80, 0, Main.maxTilesY);
            int maxYTile = (int)MathHelper.Clamp(Main.dungeonY + 80, 0, Main.maxTilesY);
            Point16 startPos = new Point16(startXTile, startYTile);
            Point16 unitY = new Point16(0, 1);
            Point16 pos = startPos;
            while (pos.X < maxXTile)
            {
                int structureIndex = WorldGen.genRand.Next(0, 2);
                Point16 structureDims = MultiStructureGenerator.GetStructureDimensions(DeadForestPlatformsPath, NeoParacosm.Instance, structureIndex);
                while (pos.Y < maxYTile)
                {
                    Tile tile = Main.tile[new Point16(pos.X + structureDims.X / 2, pos.Y)];
                    if (tile.HasTile && (tile.TileType == TileType<DeadDirtBlock>() || tile.TileType == TileID.Stone))
                    {
                        // Offset the platform vertically for more variety
                        int verticalOffset = WorldGen.genRand.Next(structureDims.Y, structureDims.Y * 2);
                        Point16 structurePos = new Point16(pos.X, pos.Y - verticalOffset);
                        MultiStructureGenerator.GenerateMultistructureSpecific(DeadForestPlatformsPath, structureIndex, structurePos, NeoParacosm.Instance);

                        Point16 wallCheckPos = structurePos + new Point16(structureDims.X / 2 - 1, structureDims.Y);

                        // Extending pillar
                        while (wallCheckPos.Y < pos.Y + 2)
                        {
                            WorldGen.PlaceWall(wallCheckPos.X, wallCheckPos.Y, WallID.AncientBlueBrickWall);
                            WorldGen.PlaceWall(wallCheckPos.X + 1, wallCheckPos.Y, WallID.AncientBlueBrickWall);
                            wallCheckPos += unitY;
                        }
                        break;
                    }
                    pos += unitY;
                }

                pos = new Point16(pos.X + WorldGen.genRand.Next(structureDims.X, structureDims.X * 2), startYTile);
                // Skip generating close to the dungeon
                if (MathF.Abs(pos.X - Main.dungeonX) < 80)
                {
                    pos = new Point16(Main.dungeonX + WorldGen.genRand.Next(81, 120), startYTile);
                }
            }
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

        void GenerateBasement()
        {
            Point16 structureDims = Generator.GetStructureDimensions(DeadForestHolyStructurePath, NeoParacosm.Instance);

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

        void GenerateHolyStructure()
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

                if (Main.tile[pointTopLeft].HasTile && Main.tile[pointTopRight].HasTile)
                {
                    attemptCount++;
                    continue;
                }

                if (!IsTileDeadDirt(pointBottomLeft) || !IsTileDeadDirt(pointBottomRight))
                {
                    attemptCount++;
                    continue;
                }

                Generator.GenerateStructure(DeadForestHolyStructurePath, new Point16(pointTopLeft), NeoParacosm.Instance);
                break;
            }
        }
    }
}
