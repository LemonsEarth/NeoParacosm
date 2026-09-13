namespace NeoParacosm.Core.Systems.World.TerrainTypes.Mountains;

/// <summary>
/// Handles procedural generation of different mountain types.
/// Each generation method creates type-specific mountains with appropriate properties.
/// </summary>
public static class MountainGenerator
{
    /// <summary>
    /// Generates a mountain with sloped ascent, flat peak, and sloped descent.
    /// </summary>
    /// <param name="startTileX">The X tile coordinate where the mountain begins.</param>
    /// <param name="width">The total width of the mountain in tiles.</param>
    /// <param name="height">The maximum height of the mountain peak in tiles (relative to surface).</param>
    /// <param name="peakWidth">The width of the peak plateau section in tiles.</param>
    /// <param name="surfaceHeights">Array containing the original surface height for each X coordinate.</param>
    /// <param name="postMountainsSurfaceHeights">Array to update with new surface heights after mountain generation. This tracks the highest point at each X coordinate.</param>
    /// <returns>A NormalMountain object containing all generation parameters and calculated peak positions.</returns>
    public static NormalMountain GenerateNormalMountain(
        int startTileX,
        int width,
        int height,
        int peakWidth,
        int[] surfaceHeights,
        int[] postMountainsSurfaceHeights)
    {
        Random rand = new Random();
        int currentMountainHeight = 0;

        // Calculate peak and slope boundaries
        int peakTileX = startTileX + (width / 2);
        int leftPeakTileX = peakTileX - peakWidth / 2;
        int rightPeakTileX = peakTileX + peakWidth / 2;
        leftPeakTileX = (int)MathHelper.Clamp(leftPeakTileX, 0, Main.maxTilesX - 1);
        rightPeakTileX = (int)MathHelper.Clamp(rightPeakTileX, 0, Main.maxTilesX - 1);

        // Ascending
        for (int i = startTileX; i < rightPeakTileX; i++)
        {
            int surfaceY = surfaceHeights[i];
            int y = surfaceY + currentMountainHeight;

            // Skip if the generated position is outside the world bounds
            if (!WorldGen.InWorld(i, y))
            {
                continue;
            }

            // Update the post-mountain surface height to reflect the new mountain top
            if (y < postMountainsSurfaceHeights[i])
            {
                postMountainsSurfaceHeights[i] = y;
            }

            // Fill all tiles from the mountain surface down to the original surface with dirt
            for (int j = y; j < surfaceY; j++)
            {
                WorldGen.PlaceTile(i, j, TileID.Dirt, true);
            }

            // Calculate height modifier based on distance from peak (creates smoother slope)
            float heightPercent = MathHelper.Clamp(MathF.Abs(currentMountainHeight) / height, 0, 1);
            float heightMul = (1 - heightPercent) + 0.5f;

            // Ascending slope: decrease height as we approach the peak
            if (i < leftPeakTileX)
            {
                currentMountainHeight -= rand.Next((int)(1 * heightMul), (int)(4 * heightMul));
            }
            else
            {
                // At peak plateau: minimal random variation to create flat top
                if (rand.Next(4) == 0)
                {
                    currentMountainHeight += rand.Next(-1, 1 + 1);
                }
            }
        }

        // Falloff to surface
        int fallOffTileX = rightPeakTileX;
        while (currentMountainHeight < 0 && fallOffTileX < Main.maxTilesX)
        {
            int surfaceY = surfaceHeights[fallOffTileX];
            int y = surfaceY + currentMountainHeight;

            // Skip if the generated position is outside the world bounds
            if (!WorldGen.InWorld(fallOffTileX, y))
            {
                y++;
                currentMountainHeight++;
                continue;
            }

            // Update the post-mountain surface height
            if (y < postMountainsSurfaceHeights[fallOffTileX])
            {
                postMountainsSurfaceHeights[fallOffTileX] = y;
            }

            // Fill tiles from mountain surface down to original surface
            for (int j = y; j < surfaceY; j++)
            {
                WorldGen.PlaceTile(fallOffTileX, j, TileID.Dirt, true);
            }

            // Calculate height modifier for smoother descent
            float heightPercent = MathHelper.Clamp(MathF.Abs(currentMountainHeight) / height, 0, 1);
            float heightMul = (1 - heightPercent) + 0.5f;

            // Increase height (reduce depth) as we move away from peak, flattening toward original surface
            currentMountainHeight += rand.Next((int)(1 * heightMul), (int)(4 * heightMul));
            fallOffTileX++;
        }

        return new NormalMountain
        {
            StartTileX = startTileX,
            Width = width,
            Height = height,
            PeakTileX = peakTileX,
            PeakWidth = peakWidth,
            PeakLeftSideTileX = leftPeakTileX,
            PeakRightSideTileX = rightPeakTileX
        };
    }

    public static SharpMountain GenerateSharpMountain(
        int startTileX,
        int width,
        int height,
        int[] surfaceHeights,
        int[] postMountainsSurfaceHeights)
    {
        Random rand = new Random();
        int currentMountainHeight = 0;

        // Calculate peak and slope boundaries
        int peakTileX = startTileX + (width / 2);

        // Ascending
        for (int i = startTileX; i < peakTileX; i++)
        {
            int surfaceY = surfaceHeights[i];
            int y = surfaceY + currentMountainHeight;

            // Skip if the generated position is outside the world bounds
            if (!WorldGen.InWorld(i, y))
            {
                continue;
            }

            // Update the post-mountain surface height to reflect the new mountain top
            if (y < postMountainsSurfaceHeights[i])
            {
                postMountainsSurfaceHeights[i] = y;
            }

            // Fill all tiles from the mountain surface down to the original surface with dirt
            for (int j = y; j < surfaceY; j++)
            {
                WorldGen.PlaceTile(i, j, TileID.Dirt, true);
            }

            // Calculate height modifier based on distance from peak (creates smoother slope)
            float heightPercent = MathHelper.Clamp(MathF.Abs(currentMountainHeight) / height, 0, 1);
            float heightMul = (1 - heightPercent) + 0.5f;

            // Ascending slope: decrease height as we approach the peak
            currentMountainHeight -= rand.Next((int)(1 * heightMul), (int)(4 * heightMul));
        }

        // Falloff to surface
        int fallOffTileX = peakTileX;
        while (currentMountainHeight < 0)
        {
            int surfaceY = surfaceHeights[fallOffTileX];
            int y = surfaceY + currentMountainHeight;

            // Skip if the generated position is outside the world bounds
            if (!WorldGen.InWorld(fallOffTileX, y))
            {
                y++;
                currentMountainHeight++;
                continue;
            }

            // Update the post-mountain surface height
            if (y < postMountainsSurfaceHeights[fallOffTileX])
            {
                postMountainsSurfaceHeights[fallOffTileX] = y;
            }

            // Fill tiles from mountain surface down to original surface
            for (int j = y; j < surfaceY; j++)
            {
                WorldGen.PlaceTile(fallOffTileX, j, TileID.Dirt, true);
            }

            // Calculate height modifier for smoother descent
            float heightPercent = MathHelper.Clamp(MathF.Abs(currentMountainHeight) / height, 0, 1);
            float heightMul = (1 - heightPercent) + 1.5f;

            // Increase height (reduce depth) as we move away from peak, flattening toward original surface
            currentMountainHeight += rand.Next((int)(1 * heightMul), (int)(4 * heightMul));
            fallOffTileX++;
        }

        return new SharpMountain
        {
            StartTileX = startTileX,
            Width = width,
            Height = height,
            PeakTileX = peakTileX,
        };
    }
}
