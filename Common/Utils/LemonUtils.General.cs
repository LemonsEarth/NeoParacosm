using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    /// <summary>
    /// <para>Creates a circle of dust around a given position.</para>
    /// <para><paramref name="noGrav"/> - if false, dust will be affected by gravity.</para>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="amount"></param>
    /// <param name="speed"></param>
    /// <param name="dustID"></param>
    /// <param name="scale"></param>
    /// <param name="noGrav"></param>
    /// <param name="alpha"></param>
    /// <param name="newColor"></param>
    public static void DustCircle(Vector2 position, int amount, float speed, int dustID, float scale = 1, bool noGrav = true, int alpha = 0, Color color = default)
    {
        for (int i = 0; i < amount; i++)
        {
            var dust = Dust.NewDustPerfect(position, dustID, newColor: color, Scale: scale);
            dust.velocity = new Vector2(0, -speed).RotatedBy(MathHelper.ToRadians(i * (360 / amount)));
            if (noGrav)
            {
                dust.noGravity = true;
            }

        }
    }

    public static void DustLine(Vector2 pos1, Vector2 pos2, int type, int distanceBetween = 16, float scale = 1, Color color = default)
    {
        Vector2 dir = pos1.DirectionTo(pos2);
        Vector2 currentPos = pos1;

        while (currentPos.Distance(pos2) > distanceBetween * 2)
        {
            Dust.NewDustPerfect(currentPos, type, Scale: scale, newColor: color).noGravity = true;
            currentPos += dir * distanceBetween * scale;
        }
    }

    /// <summary>
    /// Accelerates an entity towards a position
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="pos">The position to accelerate towards</param>
    /// <param name="xDecel">The "turning speed" on the X axis. Increase this value if you want the NPC to decelerate faster if its not moving in the desired direction</param>
    /// <param name="yDecel">Same as xDecel, just on the Y axis</param>
    /// <param name="xAccel">The desired acceleration on the X axis</param>
    /// <param name="yAccel">The desired acceleration on the Y axis</param>
    public static void MoveToPos(this Entity entity, Vector2 pos, float xDecel = 1f, float yDecel = 1f, float xAccel = 1f, float yAccel = 1f)
    {
        Vector2 direction = entity.Center.DirectionTo(pos);
        if (direction.HasNaNs())
        {
            return;
        }
        float XaccelMod = Math.Sign(direction.X) - Math.Sign(entity.velocity.X);
        float YaccelMod = Math.Sign(direction.Y) - Math.Sign(entity.velocity.Y);
        entity.velocity += new Vector2(XaccelMod * xDecel + xAccel * Math.Sign(direction.X), YaccelMod * yDecel + yAccel * Math.Sign(direction.Y));
    }

    /// <summary>
    /// Returns 1 for Small Worlds, 2 for Medium Worlds, 3 for Large Worlds (and bigger?)
    /// </summary>
    /// <returns></returns>
    public static int GetWorldSize()
    {
        switch (Main.maxTilesX)
        {
            case >= 8400:
                return 3;
            case >= 6400:
                return 2;
            default:
                return 1;
        }
    }

    /// <summary>
    /// Returns 1 for Classic and Journey, 2 for Expert, 3 for Master.
    /// Doubles value if For the Worthy seed is active
    /// </summary>
    /// <returns></returns>
    public static int GetDifficulty()
    {
        int difficulty = 1;
        if (Main.expertMode) difficulty++;
        if (Main.masterMode) difficulty++;
        if (Main.getGoodWorld) difficulty *= 2;
        return difficulty;
    }

    public static Vector2 BezierCurve(Vector2 pointA, Vector2 pointB, Vector2 controlPoint, float fracComplete)
    {
        Vector2 AToControl = Vector2.Lerp(pointA, controlPoint, fracComplete);
        Vector2 ControlToB = Vector2.Lerp(controlPoint, pointB, fracComplete);
        Vector2 finalPoint = Vector2.Lerp(AToControl, ControlToB, fracComplete);

        return finalPoint;
    }

    public static Vector2 RandomVector2Circular(float circleHalfWidth, float circleHalfHeight, float minWidth = 0, float minHeight = 0)
    {
        float width = 0;
        float height = 0;
        do
        {
            width = Main.rand.NextFloat(-circleHalfWidth, circleHalfWidth);
        }
        while (Math.Abs(width) <= minWidth);

        do
        {
            height = Main.rand.NextFloat(-circleHalfHeight, circleHalfHeight);
        }
        while (Math.Abs(height) <= minHeight);

        return new Vector2(width, height);
    }

    public static void DebugPlayerCenter(Player player)
    {
        Main.NewText("Player Center: " + player.Center);
    }

    public static void DebugPlayerTileCoords(Player player)
    {
        Main.NewText("Player Tile Coords: " + player.Center.ToTileCoordinates());
    }

    public static SpriteEffects SpriteDirectionToSpriteEffects(int spriteDirection)
    {
        if (spriteDirection == -1) return SpriteEffects.FlipHorizontally;
        return SpriteEffects.None;
    }

    public static void DrawGlow(Vector2 position, Color color, float opacity, float scale)
    {
        Main.EntitySpriteDraw(ParacosmTextures.GlowBallTexture.Value, position - Main.screenPosition, null, color * opacity, 0f, ParacosmTextures.GlowBallTexture.Size() * 0.5f, scale, SpriteEffects.None);
    }

    public static float ClosenessToMidpoint(int length, int index)
    {
        if (index >= length || index < 0)
        {
            throw new IndexOutOfRangeException();
        }
        int mid = length / 2;
        int distanceToMid = (int)MathF.Abs(index - mid);
        int closeness = 1 - (distanceToMid / mid);
        return closeness;
    }
}
