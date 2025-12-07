using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
namespace NeoParacosm.Core.CustomSkies.Biome;

public class DCSky : CustomSky
{
    public bool _isActive = false;

    public override void Update(GameTime gameTime)
    {

    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        Texture2D sky = ParacosmTextures.NoiseTexture.Value;
        float t = (float)Main.time / 256;
        spriteBatch.Draw(sky, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0.2f, 0, 0, ((MathF.Sin(t) + 3) / 4) * Opacity));
        spriteBatch.Draw(sky, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0.05f, 0, 0.2f, ((3 - MathF.Sin(t)) / 4)) * Opacity);
    }

    public override bool IsActive()
    {
        return _isActive;
    }

    public override void Activate(Vector2 position, params object[] args)
    {
        _isActive = true;
        Opacity = 0;
    }

    public override void Deactivate(params object[] args)
    {
        _isActive = false;
        Opacity = 0;
    }

    public override void Reset()
    {
        _isActive = false;
        Opacity = 0;
    }
}
