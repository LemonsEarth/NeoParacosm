using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoParacosm.Core.Systems.World.TerrainTypes.Caves;

public class Cave
{
    /// <summary>
    /// The points that make up the cave.
    /// </summary>
    public Point[] CavePoints { get; set; }

    /// <summary>
    /// The start tile X coordinate of the cave.
    /// </summary>
    public int StartTileX { get; set; } = 0;

    /// <summary>
    /// The start tile Y coordinate of the cave.
    /// </summary>
    public int StartTileY { get; set; } = 0;

    /// <summary>
    /// Function determining the width of the cave at CavePoints[i].
    /// </summary>
    public Func<int, int> CaveWidthFunc { get; set; }

    public int GetCaveWidthAtIndex(int index)
    {
        return CaveWidthFunc(index);
    }

    /// <summary>
    /// The initial direction the cave generates in.
    /// </summary>
    public Vector2 InitialDirection { get; set; }

    /// <summary>
    /// How many points the cave comprises of; the length of CavePoints.
    /// </summary>
    public int Length { get; set; } = 0;
}
