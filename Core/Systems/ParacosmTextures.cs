using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Core.Systems;

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public class ParacosmTextures : ModSystem
{
    public static Asset<Texture2D> NoiseTexture { get; private set; }
    public override void Load()
    {
        NoiseTexture = ModContent.Request<Texture2D>("NeoParacosm/Common/Assets/Textures/Noise/NoiseTexture");
    }
}
