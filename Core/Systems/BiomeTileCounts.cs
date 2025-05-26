using NeoParacosm.Content.Tiles.Depths;

namespace NeoParacosm.Core.Systems;

public class BiomeTileCounts : ModSystem
{
    public int depthStoneTileCount = 0;

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        depthStoneTileCount = tileCounts[ModContent.TileType<DepthStoneBlock>()];
    }
}
