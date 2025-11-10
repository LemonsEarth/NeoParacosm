using NeoParacosm.Content.Tiles.DeadForest;
using NeoParacosm.Content.Tiles.Depths;

namespace NeoParacosm.Core.Systems.World;

public class BiomeSystem : ModSystem
{
    public int depthStoneTileCount = 0;
    public int deadDirtTileCount = 0;

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        depthStoneTileCount = tileCounts[TileType<DepthStoneBlock>()];
        deadDirtTileCount = tileCounts[TileType<DeadDirtBlock>()];
    }
}
