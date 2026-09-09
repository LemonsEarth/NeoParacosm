namespace NeoParacosm.Core.Systems.World.TerrainTypes.Mountains;

/// <summary>
/// A mountain type with a sloped ascent, a flat peak plateau, and a sloped descent.
/// Type-specific properties for controlling the peak shape.
/// </summary>
public class NormalMountain : Mountain
{
    /// <summary>
    /// The width of the peak plateau section in tiles.
    /// </summary>
    public int PeakWidth { get; set; } = 0;

    /// <summary>
    /// The left boundary of the peak plateau in tiles.
    /// </summary>
    public int PeakLeftSideTileX { get; set; } = 0;

    /// <summary>
    /// The right boundary of the peak plateau in tiles.
    /// </summary>
    public int PeakRightSideTileX { get; set; } = 0;
}