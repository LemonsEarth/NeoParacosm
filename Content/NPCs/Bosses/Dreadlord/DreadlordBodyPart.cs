using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using ReLogic.Content;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.NPCs.Bosses.Dreadlord;

public class DreadlordBodyPart
{
    public DreadlordBodyPart()
    {

    }
    public DreadlordBodyPart(Asset<Texture2D> texture, Vector2 defaultPosition, Vector2 position, Vector2 miscPosition1, Vector2 miscPosition2, float rotation, Vector2 origin)
    {
        Texture = texture;
        DefaultPosition = defaultPosition;
        Position = position;
        MiscPosition1 = miscPosition1;
        MiscPosition2 = miscPosition2;
        Rotation = rotation;
        Origin = origin;
    }

    public Asset<Texture2D> Texture { get; set; }
    private Vector2 _defaultPosition;
    public Vector2 DefaultPosition
    {
        get
        {
            return _defaultPosition;
        }
        set
        {
            _defaultPosition = value;
            if (Position == Vector2.Zero)
            {
                Position = _defaultPosition;
            }
        }
    }
    public Vector2 Position { get; set; }
    public Vector2 MiscPosition1 { get; set; }
    public Vector2 MiscPosition2 { get; set; }
    public int Frames { get; set; } = 1;
    public int CurrentFrame { get; set; } = 0;
    public Rectangle Frame => Texture.Frame(1, Frames, 0, CurrentFrame);
    public float Rotation { get; set; } = 0f;
    public Vector2 Origin { get; set; }
    public float Scale { get; set; } = 1f;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    public float Opacity = 1f;

    public float Width => Frame.Width;
    public float Height => Frame.Height;

    public void Draw(bool useShader = false, int shaderTimer = 0)
    {
        if (Texture == null)
        {
            return;
        }
        Main.EntitySpriteDraw(Texture.Value, Position - Main.screenPosition, Frame, Color.White * Opacity, Rotation, Origin, Scale, SpriteEffects);
        if (useShader)
        {
            var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
            shader.Shader.Parameters["uTime"].SetValue(shaderTimer);
            shader.Shader.Parameters["color"].SetValue(Color.Gold.ToVector4() * Opacity);
            shader.Shader.Parameters["moveSpeed"].SetValue(1f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
            shader.Apply();
            Main.EntitySpriteDraw(Texture.Value, Position - Main.screenPosition, Frame, Color.White, Rotation, Origin, Scale, SpriteEffects);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
