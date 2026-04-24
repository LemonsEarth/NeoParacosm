using NeoParacosm.Core.Systems.Data;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Core.Conditions;

public static class LemonConditions
{
    public static Condition DownedResearcher { get; private set; } =
        new Condition(LemonUtils.GetLocKey("Conditions", "DownedResearcher"), () => ResearcherQuest.Progress >= ResearcherQuest.ProgressState.DownedResearcher);
}
