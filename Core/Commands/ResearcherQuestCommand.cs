using NeoParacosm.Core.Systems.Data;

namespace NeoParacosm.Core.Commands;

public class ResearcherQuestCommand : ModCommand
{
    public override string Command => "setResearcherQuest";

    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }

        if (args.Length != 1)
        {
            Main.NewText("Command requires 1 argument.");
            return;
        }

        if (Enum.TryParse(args[0], true, out ResearcherQuest.ProgressState state))
        {
            ResearcherQuest.Progress = state;
        }
        else
        {
            Main.NewText("Invalid progress state.");
        }
    }
}
