using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;

namespace NeoParacosm.Common.Utils.Prim
{
    public class PrimHelper
    {
        /// <summary>
        /// Draw a basic primitive trail for a projectile, from the projectile's current position to Projectile.oldPos[posIndex]
        /// </summary>
        /// <param name="projectile">The projectile to draw the trail for</param>
        /// <param name="posIndex">The final point of the trail</param>
        /// <param name="BasicEffect">BasicEffect object that should be created in a load hook</param>
        /// <param name="GraphicsDevice">Should be Main.instance.GraphicsDevice, or the one you passed in when creating BasicEffect</param>
        public static void DrawBasicProjectilePrimTrailTriangular(Projectile projectile, int posIndex, Color startColor, Color endColor, BasicEffect BasicEffect, GraphicsDevice GraphicsDevice, float scale = 1f)
        {
            Vector2 topPos = projectile.Center - (Vector2.UnitY * (projectile.height * scale / 2)).RotatedBy(projectile.velocity.ToRotation());
            Vector2 botPos = projectile.Center + (Vector2.UnitY * (projectile.height * scale / 2)).RotatedBy(projectile.velocity.ToRotation());
            Vector2 oldPos = projectile.oldPos[posIndex] != Vector2.Zero ? projectile.oldPos[posIndex] : projectile.position;
            oldPos += new Vector2(projectile.width * scale / 2, projectile.height * scale / 2);

            VertexPositionColorTexture[] vertices =
            {
                new VertexPositionColorTexture(new Vector3(topPos, 0), startColor, Vector2.Zero),
                new VertexPositionColorTexture(new Vector3(botPos, 0), startColor, Vector2.Zero),
                new VertexPositionColorTexture(new Vector3(oldPos, 0), endColor, Vector2.Zero),
            };
            BasicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
            BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Viewport viewport = GraphicsDevice.Viewport;
            BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 10);
            GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
            BasicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 1);
        }

        public static void DrawBasicProjectilePrimTrailRectangular(Projectile projectile, int posIndex, Color startColor, Color endColor, BasicEffect BasicEffect, GraphicsDevice GraphicsDevice)
        {
            Vector2 topPos = projectile.Center - (Vector2.UnitY * (projectile.height / 2)).RotatedBy(projectile.velocity.ToRotation());
            Vector2 botPos = projectile.Center + (Vector2.UnitY * (projectile.height / 2)).RotatedBy(projectile.velocity.ToRotation());
            Vector2 oldPos = projectile.oldPos[posIndex] != Vector2.Zero ? projectile.oldPos[posIndex] : projectile.position;
            oldPos += new Vector2(projectile.width / 2, projectile.height / 2);
            Vector2 oldPos1 = oldPos - (Vector2.UnitY * (projectile.height / 2)).RotatedBy(projectile.velocity.ToRotation());
            Vector2 oldPos2 = oldPos + (Vector2.UnitY * (projectile.height / 2)).RotatedBy(projectile.velocity.ToRotation());

            VertexPositionColorTexture[] vertices =
            {
                new VertexPositionColorTexture(new Vector3(topPos, 0), startColor, Vector2.Zero),
                new VertexPositionColorTexture(new Vector3(oldPos1, 0), endColor, Vector2.Zero),
                new VertexPositionColorTexture(new Vector3(botPos, 0), startColor, Vector2.Zero),
                new VertexPositionColorTexture(new Vector3(oldPos1, 0), endColor, Vector2.Zero),
                new VertexPositionColorTexture(new Vector3(botPos, 0), startColor, Vector2.Zero),
                new VertexPositionColorTexture(new Vector3(oldPos2, 0), endColor, Vector2.Zero),

            };
            BasicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
            BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Viewport viewport = GraphicsDevice.Viewport;
            BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 10);
            GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
            BasicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
        }
    }
}
