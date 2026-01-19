using NeoParacosm.Core.Systems.Data;

namespace NeoParacosm.Core.Misc.SceneEffects;

public class NoMusicSceneEffect : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Common/Assets/Audio/Music/Nothing1Minute");
    public override SceneEffectPriority Priority => (SceneEffectPriority)99999;

    public override bool IsSceneEffectActive(Player player)
    {
        return player.NPPlayer().NoMusic;
    }
}

