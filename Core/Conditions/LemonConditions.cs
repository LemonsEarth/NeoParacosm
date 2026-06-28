using NeoParacosm.Core.Systems.Data;

namespace NeoParacosm.Core.Conditions;

public static class LemonConditions
{
    public static Condition DownedResearcher { get; private set; } =
        new Condition(LemonUtils.GetLocKey("Conditions", "DownedResearcher"), () => ResearcherQuest.Progress >= ResearcherQuest.ProgressState.DownedResearcher);

    public static Condition DownedDreadlord { get; private set; } =
        new Condition(LemonUtils.GetLocKey("Conditions", "DownedDreadlord"), () => DownedBossSystem.downedDreadlord);

    public static Condition DarkCataclysmActive { get; private set; } =
        new Condition(LemonUtils.GetLocKey("Conditions", "DarkCataclysmActive"), () => DarkCataclysmSystem.DarkCataclysmActive);
}
