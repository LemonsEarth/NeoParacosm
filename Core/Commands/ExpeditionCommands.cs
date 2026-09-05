using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.Systems.Expeditions;
using SubworldLibrary;

namespace NeoParacosm.Core.Commands;

public class ExpeditionCommands : ModCommand
{
    public override string Command => "startexpedition";
    public override bool IsCaseSensitive => false;
    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }

        SubworldSystem.Enter<TestExpedition>();
    }
}
