using System.Collections.Generic;

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

    public static Cave GenerateCave(Point startTile, Vector2 initialDirection, int length, Func<int, int> caveWidthFunc, Func<int, float> caveAngleFunc)
    {
        Point[] cavePoints = new Point[length];
        Vector2 currentDirection = initialDirection;
        Point currentTile = startTile;
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
            StartTileX = startTile.X,
            StartTileY = startTile.Y,
            CaveWidthFunc = caveWidthFunc,
            InitialDirection = initialDirection,
            Length = length
        };
    }

    public static Cave GenerateCaveBetweenPoints(int startTileX, int startTileY, int endTileX, int endTileY, Func<int, int> caveWidthFunc)
    {
        Random rand = new Random();
        List<Point> cavePoints = new List<Point>();
        Vector2 startToEnd = new Vector2(endTileX - startTileX, endTileY - startTileY);
        Vector2 startToEndDirection = startToEnd.SafeNormalize(Vector2.Zero);
        float startToEndLength = startToEnd.Length();

        Point startTile = new Point(startTileX, startTileY);
        Vector2 startToEndNormal = startToEndDirection.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);

        Point currentTile = new Point(startTileX, startTileY);
        Vector2 startToCurrentTile = new Vector2(currentTile.X - startTileX, currentTile.Y - startTileY);
        float startToCurrentTileLength = startToCurrentTile.Length();

        int pointCounter = 0;
        while (startToCurrentTileLength < startToEndLength)
        {
            cavePoints.Add(currentTile);

            int caveWidth = caveWidthFunc(pointCounter);
            WorldGen.TileRunner(currentTile.X, currentTile.Y, caveWidth * 2, 10, -1);
            startToCurrentTile = new Vector2(currentTile.X - startTileX, currentTile.Y - startTileY);
            Vector2 startToCurrentTileDir = startToCurrentTile.SafeNormalize(Vector2.Zero);
            startToCurrentTileLength = startToCurrentTile.Length();

            int normalOffset = rand.Next(-caveWidth * 2, caveWidth * 2);
            if (normalOffset < 0 && startToCurrentTileDir.ToRotation() < startToEndDirection.ToRotation()
                || normalOffset > 0 && startToCurrentTileDir.ToRotation() > startToEndDirection.ToRotation())
            {
                normalOffset *= -1;
            }
            Vector2 offset = startToEndNormal * normalOffset;

            currentTile += (startToEndDirection + offset).ToPoint();


            pointCounter++;
        }
        cavePoints.Add(new Point(endTileX, endTileY));
        int caveWidthFinal = caveWidthFunc(pointCounter);
        WorldGen.TileRunner(endTileX, endTileY, caveWidthFinal * 2, 10, -1);
        WorldGen.PlaceTile(startTileX, startTileY, TileID.AmberGemspark, forced: true);
        WorldGen.PlaceTile(endTileX, endTileY, TileID.DiamondGemspark, forced: true);

        return new Cave
        {
            CavePoints = cavePoints.ToArray(),
            StartTileX = startTileX,
            StartTileY = startTileY,
            CaveWidthFunc = caveWidthFunc,
            InitialDirection = startToEnd,
            Length = cavePoints.Count
        };
    }
}
