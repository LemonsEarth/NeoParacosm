using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace NeoParacosm.Core.Systems;

public class ParacosmTextures : ModSystem
{
    public static Asset<Texture2D> NoiseTexture { get; private set; }
    public static Asset<Texture2D> TransparentNoiseTexture { get; private set; }
    public override void Load()
    {
        NoiseTexture = ModContent.Request<Texture2D>("NeoParacosm/Common/Assets/Textures/Noise/NoiseTexture");
        TransparentNoiseTexture = ModContent.Request<Texture2D>("NeoParacosm/Common/Assets/Textures/Noise/TransparentNoise");
    }
}
