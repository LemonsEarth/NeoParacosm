using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
namespace NeoParacosm.Core.CustomSkies.Biome;

public class DCSky : CustomSky
{
    public bool _isActive = false;

    public override void Update(GameTime gameTime)
    {
        Opacity = MathHelper.Lerp(Opacity, 0.5f, 1f / 60f);
    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        Texture2D sky = TextureAssets.MagicPixel.Value;
        spriteBatch.Draw(sky, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(15 / 255f, 0, 14 / 255f, Opacity));
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
