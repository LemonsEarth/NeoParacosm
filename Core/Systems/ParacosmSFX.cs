using Terraria.Audio;

namespace NeoParacosm.Core.Systems;

public class ParacosmSFX : ModSystem
{
    public static SoundStyle UndertakerGunshot { get; private set; }
    public override void Load()
    {
        UndertakerGunshot = new SoundStyle("NeoParacosm/Common/Assets/Audio/SFX/UndertakerGunshot");
    }
}
