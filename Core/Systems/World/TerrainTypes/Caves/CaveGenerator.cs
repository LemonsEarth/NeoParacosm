namespace NeoParacosm.Core.Systems.World.TerrainTypes.Caves;

public class CaveGenerator
{
    public static float LinearCaveAngleFunc(float i, int length, float startAngle, float endAngle)
    {
        float lerpT = i / (length - 1);
        return MathHelper.Lerp(startAngle, endAngle, lerpT);
    }

    public static float LinearCaveAngleFuncWithRandomSharpTurn(float i, int length, float startAngle, float endAngle, int sharpTurnDenominator)
    {
        if (Main.rand.NextBool(sharpTurnDenominator))
        {
            return MathHelper.PiOver2 * Main.rand.NextBool().ToDirectionInt();
        }
        float lerpT = i / (length - 1);
        return MathHelper.Lerp(startAngle, endAngle, lerpT);
    }

    static void MakeHole(int x, int y, int radius)
    {
        for (int i = -radius; i < radius; i++)
        {
            for (int j = -radius; j < radius; j++)
            {
                int movedX = x + i;
                int movedY = y + j;
                int adjustedRadius = radius;
                if (Main.rand.NextBool(10))
                {
                    adjustedRadius += Main.rand.Next(-2, 2);
                }
                if (new Vector2(x, y).DistanceSQ(new Vector2(movedX, movedY)) < adjustedRadius * adjustedRadius)
                {
                    if (movedX > 0 && movedX < Main.maxTilesX && movedY > 0 && movedY < Main.maxTilesY)
                    {
                        WorldGen.KillTile(movedX, movedY, false, false, true);
                    }
                }
            }
        }
    }

    public static Cave GenerateCave(int startTileX, int startTileY, Vector2 initialDirection, int length, Func<int, int> caveWidthFunc, Func<int, float> caveAngleFunc)
    {
        Point[] cavePoints = new Point[length];
        Vector2 currentDirection = initialDirection;
        Point currentTile = new Point(startTileX, startTileY);
        for (int i = 0; i < length; i++)
        {
            cavePoints[i] = currentTile;
            int caveWidth = caveWidthFunc(i);
            WorldGen.TileRunner(currentTile.X, currentTile.Y, caveWidth * 2, 10, -1);
            float randomAngle = caveAngleFunc(i);

            currentDirection = currentDirection.RotatedBy(Main.rand.NextFloat(-randomAngle, randomAngle));

            currentTile += (currentDirection * caveWidth).ToPoint();
        }

        return new Cave
        {
            CavePoints = cavePoints,
            StartTileX = startTileX,
            StartTileY = startTileY,
            CaveWidthFunc = caveWidthFunc,
            InitialDirection = initialDirection,
            Length = length
        };
    }
}
