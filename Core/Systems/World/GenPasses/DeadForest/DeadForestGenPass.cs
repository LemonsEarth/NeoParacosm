using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.DeadForest;

public class DeadForestGenPass : GenPass
{
    public DeadForestGenPass(string name, float loadWeight) : base(name, loadWeight) { }

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
    }
}
