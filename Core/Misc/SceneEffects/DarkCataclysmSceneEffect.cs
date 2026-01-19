using NeoParacosm.Core.Systems.Data;

namespace NeoParacosm.Core.Misc.SceneEffects;

public class DarkCataclysmSceneEffect : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Common/Assets/Audio/Music/DarkCataclysm");
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

    public override bool IsSceneEffectActive(Player player)
    {
        return ResearcherQuest.DarkCataclysmActive;
    }
}

