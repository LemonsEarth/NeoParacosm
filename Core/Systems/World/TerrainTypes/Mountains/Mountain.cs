namespace NeoParacosm.Core.Systems.World.TerrainTypes.Mountains;

/// <summary>
/// Base class for all mountain types. Contains common properties shared across all mountains.
/// </summary>
public abstract class Mountain
{
    /// <summary>
    /// The X tile coordinate where the mountain begins.
    /// </summary>
    public int StartTileX { get; set; } = 0;

    /// <summary>
    /// The Y tile coordinate where the mountain's base starts (surface level).
    /// </summary>
    public int StartTileY { get; set; } = 0;

    /// <summary>
    /// The final X coordinate of the mountain.
    /// </summary>
    public int EndTileX => StartTileX + Width;

    /// <summary>
    /// The Y tile coordinate where the mountain's base ends.
    /// </summary>
    public int EndTileY { get; set; } = 0;

    /// <summary>
    /// The total width of the mountain in tiles.
    /// </summary>
    public int Width { get; set; } = 0;

    /// <summary>
    /// The mountain's height. Note that it may not be exact, as it is primarily used as a factor in generation code.
    /// </summary>
    public int Height { get; set; } = 0;

    /// <summary>
    /// The X tile coordinate of the mountain's peak.
    /// </summary>
    public int PeakTileX { get; set; } = 0;

    /// <summary>
    /// Controls the steepness of slopes. Higher values = steeper slopes.
    /// </summary>
    public float Steepness { get; set; } = 1.0f;
}