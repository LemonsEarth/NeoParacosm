using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.UI;
using Terraria.DataStructures;
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
        if (color == default) color = Color.White;
        Vector2 dir = pos1.DirectionTo(pos2);
        float distance = pos1.Distance(pos2);
        int dustCount = (int)(distance / 16);
        Vector2 currentPos = pos1;
        while (dustCount > 0)
        {
            Dust.NewDustPerfect(currentPos, type, Scale: scale, newColor: color).noGravity = true;
            currentPos += dir * distanceBetween;
            dustCount--;
        }
    }

    public static void SmokeGore(IEntitySource source, Vector2 pos, float minSpeed, float maxSpeed)
    {
        if (!Main.dedServ)
        {
            Gore.NewGoreDirect(source, pos, Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(minSpeed, maxSpeed), Main.rand.NextFromList(GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3));
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

    public static Projectile QuickPulse(Entity sourceEntity, Vector2 pos, float speed, float scale, float colorMult, Color? color = null, Entity entityToFollow = null)
    {
        if (color == null)
        {
            color = Color.White;
        }
        Projectile proj = Projectile.NewProjectileDirect(
            sourceEntity.GetSource_FromThis(),
            pos,
            Vector2.Zero,
            ProjectileType<PulseEffect>(),
            0,
            0,
            -1,
            speed, scale, colorMult
            );
        PulseEffect pulse = proj.ModProjectile as PulseEffect;
        pulse.PulseColor = color.Value;
        pulse.EntityToFollow = entityToFollow;
        return proj;
    }

    public static void DrawLaser(Vector2 position1, Vector2 position2, float laserWidth, Color color)
    {
        Main.EntitySpriteDraw(ParacosmTextures.TrueMagicPixel.Value,
            position1 - Main.screenPosition,
            null,
            color,
            position1.DirectionTo(position2).ToRotation(),
            new Vector2(0, ParacosmTextures.TrueMagicPixel.Height() * 0.5f),
            new Vector2((position1 - Main.screenPosition).Distance(position2 - Main.screenPosition) / 4, 1),
            SpriteEffects.None);
    }

    public static Projectile QuickLaser(Entity sourceEntity, Vector2 position, Vector2 endPositionOffset, int duration, float laserWidth, Color color, Entity entityToFollow = null)
    {
        Projectile proj = QuickProj(
            sourceEntity,
            position,
            endPositionOffset,
            ProjectileType<LaserEffect>(),
            0,
            ai0: duration,
            ai1: laserWidth
            );

        LaserEffect laser = proj.ModProjectile as LaserEffect;
        laser.LaserColor = color;
        laser.EntityToFollow = entityToFollow;
        return proj;
    }

    public static void QuickCameraFocus(Vector2 position, Func<bool> endConditionFunc)
    {
        MoveCameraModifier cameraModifier = new MoveCameraModifier(position, endConditionFunc);
        Main.instance.CameraModifiers.Add(cameraModifier);
    }
}
