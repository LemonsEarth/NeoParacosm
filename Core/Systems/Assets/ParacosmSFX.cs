using Terraria.Audio;

namespace NeoParacosm.Core.Systems.Assets;

public class ParacosmSFX : ModSystem
{
    public static SoundStyle UndertakerGunshot { get; private set; }
    public static SoundStyle ElectricBurst { get; private set; }
    public static SoundStyle ChurchBell { get; private set; }
    public override void Load()
    {
        UndertakerGunshot = new SoundStyle("NeoParacosm/Common/Assets/Audio/SFX/UndertakerGunshot");
        ElectricBurst = new SoundStyle("NeoParacosm/Common/Assets/Audio/SFX/ElectricBurst");
        ChurchBell = new SoundStyle("NeoParacosm/Common/Assets/Audio/SFX/ChurchBell");
    }
}
