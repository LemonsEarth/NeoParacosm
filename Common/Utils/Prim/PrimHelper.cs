using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Common.Utils.Prim;

public class PrimHelper
{
    public static GraphicsDevice GraphicsDevice => Main.instance.GraphicsDevice;
    public static Viewport GDViewport => GraphicsDevice.Viewport;

    /// <summary>
    /// Draws a triangular prim trail for the given projectile. BasicEffect needs to be loaded in on the Main Thread (see existing projectiles with trails).
    /// <paramref name="topVertexRotation"/> is the rotation of the top point of the trail. If null, it'll take the rotation of the projectile's velocity, rotated by -90 degrees.
    /// <paramref name="bottomVertexRotation"/> is the rotation of the bottom point of the trail. If null, it'll take the rotation of the projectile's velocity, rotated by 90 degrees.
    /// <paramref name="topDistance"/> is the distance of the top point of the trail from the center. If null, it'll take half of the height of the projectile's texture.
    /// <paramref name="bottomDistance"/> is the distance of the bottom point of the trail from the center. If null, it'll take half of the height of the projectile's texture.
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="startColor"></param>
    /// <param name="endColor"></param>
    /// <param name="BasicEffect"></param>
    /// <param name="topVertexRotation"></param>
    /// <param name="bottomVertexRotation"></param>
    /// <param name="topDistance"></param>
    /// <param name="bottomDistance"></param>
    public static void DrawBasicProjectilePrimTrailTriangular(Projectile projectile, Color startColor, Color endColor, BasicEffect BasicEffect, float? topVertexRotation = null, float? bottomVertexRotation = null, int? topDistance = null, int? bottomDistance = null, Vector2? positionOffset = null)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        for (int i = 0; i < projectile.oldPos.Length; i++)
        {
            Vector2 currentPos = projectile.oldPos[i];
            int oldPosIndex = i + 1 >= projectile.oldPos.Length ? i : i + 1;
            Vector2 oldPos = projectile.oldPos[oldPosIndex];
            if (currentPos == Vector2.Zero || oldPos == Vector2.Zero)
            {
                break;
            }

            float lerpT = (float)i / projectile.oldPos.Length;
            float oldLerpT = (float)oldPosIndex / projectile.oldPos.Length;

            float topRot = topVertexRotation ?? projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float botRot = bottomVertexRotation ?? projectile.velocity.ToRotation() + MathHelper.PiOver2;

            int topVertexDistance = topDistance ?? (texture.Height / 2);
            int botVertexDistance = bottomDistance ?? (texture.Height / 2);
            Vector2 topVertexOffset = Vector2.UnitX.RotatedBy(topRot) * topVertexDistance * (1 - lerpT);
            Vector2 botVertexOffset = Vector2.UnitX.RotatedBy(botRot) * botVertexDistance * (1 - lerpT);

            Vector2 posOffset = positionOffset ?? new Vector2(texture.Width / 2, texture.Height / 2);
            currentPos += posOffset;
            oldPos += posOffset;


            Color colorFront = Color.Lerp(startColor, endColor, lerpT);
            Color colorBack = Color.Lerp(startColor, endColor, oldLerpT);

            VertexPositionColorTexture topVPCT = QuickVertexPositionColorTexture(currentPos + topVertexOffset, colorFront);
            VertexPositionColorTexture botVPCT = QuickVertexPositionColorTexture(currentPos + botVertexOffset, colorFront);
            VertexPositionColorTexture oldTopVPCT = QuickVertexPositionColorTexture(oldPos + topVertexOffset, colorBack);
            VertexPositionColorTexture oldBotVPCT = QuickVertexPositionColorTexture(oldPos + botVertexOffset, colorBack);
            vertices.Add(topVPCT);
            vertices.Add(botVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(topVPCT);
            vertices.Add(oldTopVPCT);
        }

        if (vertices.Count == 0)
        {
            return;
        }

        DrawPrimitives(BasicEffect, vertices);
    }

    /// <summary>
    /// Creates a basic trail for the given projectile
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="startColor"></param>
    /// <param name="endColor"></param>
    /// <param name="BasicEffect"></param>
    /// <param name="topVertexRotation"></param>
    /// <param name="bottomVertexRotation"></param>
    public static void DrawBasicProjectilePrimTrailRectangular(Projectile projectile, Color startColor, Color endColor, BasicEffect BasicEffect, float? topVertexRotation = null, float? bottomVertexRotation = null, int? topDistance = null, int? bottomDistance = null)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        Vector2 moveDirection = projectile.velocity.SafeNormalize(Vector2.Zero);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float topRot = topVertexRotation ?? projectile.velocity.ToRotation() - MathHelper.PiOver2;
        float botRot = bottomVertexRotation ?? projectile.velocity.ToRotation() + MathHelper.PiOver2;
        int topVertexDistance = topDistance ?? (texture.Height / 2);
        int botVertexDistance = bottomDistance ?? (texture.Height / 2);
        Vector2 topVertexOffset = Vector2.UnitX.RotatedBy(topRot) * topVertexDistance;
        Vector2 botVertexOffset = Vector2.UnitX.RotatedBy(botRot) * botVertexDistance;
        for (int i = 0; i < projectile.oldPos.Length; i++)
        {
            Vector2 currentPos = projectile.oldPos[i];
            int oldPosIndex = i + 1 >= projectile.oldPos.Length ? i : i + 1;
            Vector2 oldPos = projectile.oldPos[oldPosIndex];
            if (currentPos == Vector2.Zero || oldPos == Vector2.Zero)
            {
                break;
            }


            currentPos += new Vector2(texture.Width / 2, texture.Height / 2);
            oldPos += new Vector2(texture.Width / 2, texture.Height / 2);

            Color colorFront = Color.Lerp(startColor, endColor, (float)i / projectile.oldPos.Length);
            Color colorBack = Color.Lerp(startColor, endColor, (float)oldPosIndex / projectile.oldPos.Length);

            VertexPositionColorTexture topVPCT = QuickVertexPositionColorTexture(currentPos + topVertexOffset, colorFront);
            VertexPositionColorTexture botVPCT = QuickVertexPositionColorTexture(currentPos + botVertexOffset, colorFront);
            VertexPositionColorTexture oldTopVPCT = QuickVertexPositionColorTexture(oldPos + topVertexOffset, colorBack);
            VertexPositionColorTexture oldBotVPCT = QuickVertexPositionColorTexture(oldPos + botVertexOffset, colorBack);
            vertices.Add(topVPCT);
            vertices.Add(botVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(topVPCT);
            vertices.Add(oldTopVPCT);
        }

        DrawPrimitives(BasicEffect, vertices);
    }

    public static void DrawHeldProjectilePrimTrailRectangular(Projectile projectile, Color startColor, Color endColor, BasicEffect BasicEffect, float topVertexRotationOffset, float bottomVertexRotationOffset, int? topDistance = null, int? bottomDistance = null)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        for (int i = 0; i < projectile.oldPos.Length; i++)
        {
            Vector2 currentPos = projectile.oldPos[i];
            int oldPosIndex = (i + 1 >= projectile.oldPos.Length) ? i : i + 1;
            Vector2 oldPos = projectile.oldPos[oldPosIndex];
            if (currentPos == Vector2.Zero || oldPos == Vector2.Zero)
            {
                break;
            }

            float topRot = topVertexRotationOffset + projectile.oldRot[i];
            float botRot = bottomVertexRotationOffset + projectile.oldRot[i];

            float oldTopRot = topVertexRotationOffset + projectile.oldRot[oldPosIndex];
            float oldBotRot = bottomVertexRotationOffset + projectile.oldRot[oldPosIndex];
            int topVertexDistance = topDistance ?? (texture.Height / 2);
            int botVertexDistance = bottomDistance ?? (texture.Height / 2);
            Vector2 topVertexOffset = Vector2.UnitX.RotatedBy(topRot) * topVertexDistance;
            Vector2 botVertexOffset = Vector2.UnitX.RotatedBy(botRot) * botVertexDistance;
            Vector2 oldTopVertexOffset = Vector2.UnitX.RotatedBy(oldTopRot) * topVertexDistance;
            Vector2 oldBotVertexOffset = Vector2.UnitX.RotatedBy(oldBotRot) * botVertexDistance;

            currentPos += new Vector2(texture.Width / 2, texture.Height / 2);
            oldPos += new Vector2(texture.Width / 2, texture.Height / 2);

            Color colorFront = Color.Lerp(startColor, endColor, (float)i / projectile.oldPos.Length);
            Color colorBack = Color.Lerp(startColor, endColor, (float)oldPosIndex / projectile.oldPos.Length);

            VertexPositionColorTexture topVPCT = QuickVertexPositionColorTexture(currentPos + topVertexOffset, colorFront);
            VertexPositionColorTexture botVPCT = QuickVertexPositionColorTexture(currentPos + botVertexOffset, colorFront);
            VertexPositionColorTexture oldTopVPCT = QuickVertexPositionColorTexture(oldPos + oldTopVertexOffset, colorBack);
            VertexPositionColorTexture oldBotVPCT = QuickVertexPositionColorTexture(oldPos + oldBotVertexOffset, colorBack);
            vertices.Add(topVPCT);
            vertices.Add(botVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(topVPCT);
            vertices.Add(oldTopVPCT);
        }

        DrawPrimitives(BasicEffect, vertices);
    }

    public static void DrawHeldProjectilePrimTrailTriangular(Projectile projectile, Color startColor, Color endColor, BasicEffect BasicEffect, float topVertexRotationOffset, float bottomVertexRotationOffset, int? topDistance = null, int? bottomDistance = null)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        for (int i = 0; i < projectile.oldPos.Length; i++)
        {
            Vector2 currentPos = projectile.oldPos[i];
            int oldPosIndex = (i + 1 >= projectile.oldPos.Length) ? i : i + 1;
            Vector2 oldPos = projectile.oldPos[oldPosIndex];
            if (currentPos == Vector2.Zero || oldPos == Vector2.Zero)
            {
                break;
            }

            float lerpT = (float)i / projectile.oldPos.Length;
            float oldLerpT = (float)oldPosIndex / projectile.oldPos.Length;

            float topRot = topVertexRotationOffset + projectile.oldRot[i];
            float botRot = bottomVertexRotationOffset + projectile.oldRot[i];

            float oldTopRot = topVertexRotationOffset + projectile.oldRot[oldPosIndex];
            float oldBotRot = bottomVertexRotationOffset + projectile.oldRot[oldPosIndex];
            int topVertexDistance = topDistance ?? (texture.Height / 2);
            int botVertexDistance = bottomDistance ?? (texture.Height / 2);
            Vector2 topVertexOffset = Vector2.UnitX.RotatedBy(topRot) * topVertexDistance * (1 - lerpT);
            Vector2 botVertexOffset = Vector2.UnitX.RotatedBy(botRot) * botVertexDistance * (1 - lerpT);
            Vector2 oldTopVertexOffset = Vector2.UnitX.RotatedBy(oldTopRot) * topVertexDistance * (1 - oldLerpT);
            Vector2 oldBotVertexOffset = Vector2.UnitX.RotatedBy(oldBotRot) * botVertexDistance * (1 - oldLerpT);

            currentPos += new Vector2(texture.Width / 2, texture.Height / 2);
            oldPos += new Vector2(texture.Width / 2, texture.Height / 2);

            Color colorFront = Color.Lerp(startColor, endColor, lerpT);
            Color colorBack = Color.Lerp(startColor, endColor, oldLerpT);

            VertexPositionColorTexture topVPCT = QuickVertexPositionColorTexture(currentPos + topVertexOffset, colorFront);
            VertexPositionColorTexture botVPCT = QuickVertexPositionColorTexture(currentPos + botVertexOffset, colorFront);
            VertexPositionColorTexture oldTopVPCT = QuickVertexPositionColorTexture(oldPos + oldTopVertexOffset, colorBack);
            VertexPositionColorTexture oldBotVPCT = QuickVertexPositionColorTexture(oldPos + oldBotVertexOffset, colorBack);
            vertices.Add(topVPCT);
            vertices.Add(botVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(topVPCT);
            vertices.Add(oldTopVPCT);
        }

        DrawPrimitives(BasicEffect, vertices);
    }


    public static VertexPositionColorTexture QuickVertexPositionColorTexture(Vector2 position, Color color, Vector2 textureCoords)
    {
        return new VertexPositionColorTexture(new Vector3(position, 0), color, textureCoords);
    }

    public static VertexPositionColorTexture QuickVertexPositionColorTexture(Vector2 position, Color color)
    {
        return new VertexPositionColorTexture(new Vector3(position, 0), color, Vector2.Zero);
    }

    public static void DrawPrimitives(BasicEffect BasicEffect, VertexPositionColorTexture[] vertices)
    {
        if (vertices.Length == 0)
        {
            return;
        }

        BasicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
        BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GDViewport.Width, GDViewport.Height, 0, -1, 10);
        GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
        BasicEffect.CurrentTechnique.Passes[0].Apply();
        GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
    }

    public static void DrawPrimitives(BasicEffect BasicEffect, List<VertexPositionColorTexture> vertices)
    {
        if (vertices.Count == 0)
        {
            return;
        }

        BasicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
        BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GDViewport.Width, GDViewport.Height, 0, -1, 10);
        GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
        BasicEffect.CurrentTechnique.Passes[0].Apply();
        GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
    }
}
