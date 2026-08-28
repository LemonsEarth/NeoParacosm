using NeoParacosm.Core.Systems.Data;

namespace NeoParacosm.Core.Commands;

public class DarkCataclysmCommand : ModCommand
{
    public override string Command => "toggleDarkCataclysm";
    public override bool IsCaseSensitive => false;
    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }

        DarkCataclysmSystem.DarkCataclysmActive = !DarkCataclysmSystem.DarkCataclysmActive;
    }
}
