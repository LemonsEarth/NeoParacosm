namespace NeoParacosm.Core.Systems.World.TerrainTypes.SurfaceTerrain;

public class SurfaceTerrainGenerator
{
    public static int AverageSurfaceLevel => (int)Main.worldSurface - 50;

    /// <summary>
    /// Generates a dirt surface, horizontally starting at startTileX and ending at endTileX, with elevation around surfaceLevel.
    /// Has a one in elevationChangeChanceDenominator chance to change elevation by a random amount between elevationChangeMin and elevationChangeMax.
    /// Fills the area below the surface with dirt tiles, down to Main.worldSurface.
    /// </summary>
    /// <param name="surfaceLevel">The y tile coordinate where the surface will generate. If null, uses SurfaceGenerator.AverageSurfaceLevel.</param>
    /// <param name="startTileX">The start x tile coordinate.</param>
    /// <param name="endTileX">The end x tile coordinate. If null, takes the current Main.maxTilesX.</param>
    /// <param name="elevationChangeChanceDenominator">The denominator for the chance to change elevation.</param>
    /// <param name="elevationChangeMin">The minimum amount to change elevation.</param>
    /// <param name="elevationChangeMax">The maximum amount to change elevation.</param>
    /// <returns></returns>
    public static SurfaceTerrain GenerateDirtSurface(int? surfaceLevel = null, int startTileX = 0, int? endTileX = null,
        int elevationChangeChanceDenominator = 4, int elevationChangeMin = 1, int elevationChangeMax = 1)
    {
        int currentY = surfaceLevel ?? AverageSurfaceLevel;

        int maxTilesXValue = endTileX ?? Main.maxTilesX;
        SurfaceTerrain surface = new SurfaceTerrain(maxTilesXValue);

        for (int i = startTileX; i < maxTilesXValue; i++)
        {
            for (int j = currentY; j < Main.worldSurface; j++)
            {
                WorldGen.PlaceTile(i, j, TileID.Dirt, true);
            }
            surface.SurfaceHeights[i] = currentY;
            if (Main.rand.NextBool(elevationChangeChanceDenominator))
            {
                currentY += Main.rand.Next(-elevationChangeMin, elevationChangeMax + 1);
            }
        }
        return surface;
    }

    public static void GrowGrassOnSurface(int highestSurfacePoint)
    {
        GrowGrass(0, highestSurfacePoint, Main.maxTilesX, (int)Main.worldSurface);
    }

    public static void GrowGrass(int startTileX = 0, int startTileY = 0, int? endTileX = null, int? endTileY = null)
    {
        int endTileXValue = endTileX ?? Main.maxTilesX;
        int endTileYValue = endTileY ?? (int)Main.worldSurface;
        for (int i = startTileX; i < endTileXValue; i++)
        {
            for (int j = startTileY; j < endTileYValue; j++)
            {
                if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.Dirt)
                {
                    if (LemonUtils.TileIsExposedToAir(i, j))
                    {
                        WorldGen.PlaceTile(i, j, TileID.Grass);
                    }
                    else
                    {
                        Main.tile[i, j].WallType = WallID.Dirt;
                    }
                }
            }
        }
    }
}
