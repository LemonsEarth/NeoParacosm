using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.UI;
using Terraria.Graphics.CameraModifiers;

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

    public static Vector2 BezierCurve(Vector2 pointA, Vector2 pointB, Vector2 controlPoint, float fracComplete)
    {
        Vector2 AToControl = Vector2.Lerp(pointA, controlPoint, fracComplete);
        Vector2 ControlToB = Vector2.Lerp(controlPoint, pointB, fracComplete);
        Vector2 finalPoint = Vector2.Lerp(AToControl, ControlToB, fracComplete);

        return finalPoint;
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

    public static void QuickScreenShake(Vector2 pos, float strength, float vibrationCyclesPerSecond, int frames, float distanceFalloff)
    {
        PunchCameraModifier mod = new PunchCameraModifier(
            pos, 
            Vector2.UnitY.RotatedByRandom(MathHelper.Pi * 2), 
            strength, 
            vibrationCyclesPerSecond, 
            frames, 
            distanceFalloff);
        Main.instance.CameraModifiers.Add(mod);
    }

    public static void QuickPulse(Entity sourceEntity, Vector2 pos, float speed, float scale, float colorMult)
    {
        Projectile.NewProjectileDirect(
            sourceEntity.GetSource_FromThis(),
            pos,
            Vector2.Zero,
            ProjectileType<PulseEffect>(),
            0,
            0,
            -1,
            speed, scale, colorMult
            );
    }

    public static void QuickCameraFocus(Vector2 position, Func<bool> endConditionFunc)
    {
        MoveCameraModifier cameraModifier = new MoveCameraModifier(position, endConditionFunc);
        Main.instance.CameraModifiers.Add(cameraModifier);
    }
}
