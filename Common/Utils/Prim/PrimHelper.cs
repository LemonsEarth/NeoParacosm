using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace NeoParacosm.Common.Utils.Prim;

public class PrimHelper
{
    public static GraphicsDevice GraphicsDevice => Main.instance.GraphicsDevice;
    public static Viewport GDViewport => GraphicsDevice.Viewport;

    /// <summary>
    /// Draw a basic primitive trail for a projectile, from the projectile's current position to Projectile.oldPos[posIndex]
    /// </summary>
    /// <param name="projectile">The projectile to draw the trail for</param>
    /// <param name="posIndex">The final point of the trail</param>
    /// <param name="BasicEffect">BasicEffect object that should be created in a load hook</param>
    /// <param name="GraphicsDevice">Should be Main.instance.GraphicsDevice, or the one you passed in when creating BasicEffect</param>
    public static void DrawBasicProjectilePrimTrailTriangular(Projectile projectile, Color startColor, Color endColor, BasicEffect BasicEffect)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        Vector2 moveDirection = projectile.velocity.SafeNormalize(Vector2.Zero);
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

            Vector2 topVertexOffset = moveDirection.RotatedBy(-MathHelper.PiOver2) * (texture.Height / 2) * (1 - lerpT);
            Vector2 botVertexOffset = moveDirection.RotatedBy(MathHelper.PiOver2) * (texture.Height / 2) * (1 - lerpT);
            currentPos += new Vector2(texture.Width / 2, texture.Height / 2);
            oldPos += new Vector2(texture.Width / 2, texture.Height / 2);


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

        BasicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
        BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GDViewport.Width, GDViewport.Height, 0, -1, 10);
        GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
        BasicEffect.CurrentTechnique.Passes[0].Apply();
        GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
    }

    public static void DrawBasicProjectilePrimTrailRectangular(Projectile projectile, Color startColor, Color endColor, BasicEffect BasicEffect)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        Vector2 moveDirection = projectile.velocity.SafeNormalize(Vector2.Zero);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        Vector2 topVertexOffset = moveDirection.RotatedBy(-MathHelper.PiOver2) * texture.Height / 2;
        Vector2 botVertexOffset = moveDirection.RotatedBy(MathHelper.PiOver2) * texture.Height / 2;
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

    public static VertexPositionColorTexture QuickVertexPositionColorTexture(Vector2 position, Color color)
    {
        return new VertexPositionColorTexture(new Vector3(position, 0), color, Vector2.Zero);
    }
}
