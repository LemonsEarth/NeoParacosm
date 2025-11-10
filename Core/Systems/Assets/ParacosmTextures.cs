using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace NeoParacosm.Core.Systems.Assets;

public class ParacosmTextures : ModSystem
{
    public static Asset<Texture2D> NoiseTexture { get; private set; }
    public static Asset<Texture2D> TransparentNoiseTexture { get; private set; }
    public static Asset<Texture2D> GlowBallTexture { get; private set; }
    public override void Load()
    {
        NoiseTexture = Request<Texture2D>("NeoParacosm/Common/Assets/Textures/Noise/NoiseTexture");
        TransparentNoiseTexture = Request<Texture2D>("NeoParacosm/Common/Assets/Textures/Noise/TransparentNoise");
        GlowBallTexture = Request<Texture2D>("NeoParacosm/Common/Assets/Textures/Misc/GlowBall");
    }
}
