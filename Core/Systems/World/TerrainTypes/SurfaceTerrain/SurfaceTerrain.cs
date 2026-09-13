namespace NeoParacosm.Core.Systems.World.TerrainTypes.SurfaceTerrain;

public class SurfaceTerrain
{
    public SurfaceTerrain(int maxTilesX, int initialSurfaceLevel)
    {
        InitialLevel = initialSurfaceLevel;
        Heights = new int[maxTilesX];
    }

    public int[] Heights { get; set; }
    public int InitialLevel { get; set; }
}
