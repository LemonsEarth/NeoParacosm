using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.DeadForest;

public class DeadForestPlatformsGenPass : GenPass
{
    public DeadForestPlatformsGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string DeadForestPlatformsPath = "Common/Assets/Structures/DeadForestPlatforms";
    int baseDeadForestTileRadius = 200;
    int DeadForestRadius => baseDeadForestTileRadius * LemonUtils.GetWorldSize();

    bool IsTileDeadDirt(Point point)
    {
        return Main.tile[point].HasTile && Main.tile[point].TileType == TileType<DeadDirtBlock>();
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateDeadForestPlatforms();
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
}
