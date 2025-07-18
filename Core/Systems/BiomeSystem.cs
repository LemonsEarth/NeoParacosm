﻿using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Tiles.DeadForest;
using NeoParacosm.Content.Tiles.Depths;

namespace NeoParacosm.Core.Systems;

public class BiomeSystem : ModSystem
{
    public int depthStoneTileCount = 0;
    public int deadDirtTileCount = 0;

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        depthStoneTileCount = tileCounts[ModContent.TileType<DepthStoneBlock>()];
        deadDirtTileCount = tileCounts[ModContent.TileType<DeadDirtBlock>()];
    }
}
