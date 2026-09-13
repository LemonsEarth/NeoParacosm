namespace NeoParacosm.Core.Systems.World.TerrainTypes.SurfaceTerrain;

public class SurfaceTerrain
{
    public SurfaceTerrain(int maxTilesX)
    {
        SurfaceHeights = new int[maxTilesX];
    }

    public int[] SurfaceHeights { get; set; }
    public int SurfaceLevel { get; set; }
}
