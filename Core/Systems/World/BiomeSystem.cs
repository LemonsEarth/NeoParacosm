using NeoParacosm.Content.Tiles.DeadForest;
using NeoParacosm.Content.Tiles.Depths;
using NeoParacosm.Core.Systems.Data;

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

    public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
    {
        if (ResearcherQuest.Progress == ResearcherQuest.ProgressState.DownedResearcher)
        {
            tileColor = new Color(90 / 255f, 6 / 255f, 82 / 255f, 1);
        }
    }
}
