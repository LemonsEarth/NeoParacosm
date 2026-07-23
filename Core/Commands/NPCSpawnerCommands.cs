using NeoParacosm.Content.Items.Misc;
using System.Linq;

namespace NeoParacosm.Core.Commands;

public class SetNPCSpawnerCommand : ModCommand
{
    public override string Command => "setNPCSpawner";

    public override CommandType Type => CommandType.Chat;

    public override bool IsCaseSensitive => true;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }

        if (args.Length < 2)
        {
            Main.NewText("Command requires at least 2 arguments.");
            return;
        }

        // Mod

        string modName = args[0];

        string npcName = args[1];
        if (npcName == null || npcName.Length == 0)
        {
            Main.NewText("Invalid NPC name.");
            return;
        }

        // Vanilla NPCs don't have "Terraria/" prefix
        string internalNPCName = modName == "Terraria" ? npcName : $"{modName}/{npcName}";

        if (NPCID.Search.TryGetId(internalNPCName, out int id))
        {
            caller.Player.GetModPlayer<NPCSpawnerPlayer>().CurrentNPCType = id;

            Main.NewText($"Selected NPC spawner ID set to {id}");
        }
        else
        {
            Main.NewText("NPC not found.");
            return;
        }

        for (int i = 2; i < args.Length; i++)
        {
            if (float.TryParse(args[i], out float argValue))
            {
                caller.Player.GetModPlayer<NPCSpawnerPlayer>().AdditionalData[i - 2] = argValue;
            }
            else
            {
                Main.NewText($"Argument #{i} could not be parsed to float.");
                return;
            }
        }
    }
}

public class ResetNPCSpawnerCommand : ModCommand
{
    public override string Command => "resetNPCSpawner";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }

        caller.Player.GetModPlayer<NPCSpawnerPlayer>().Reset();
        Main.NewText("Reset NPC spawner placer.");
    }
}

public class ToggleNPCSpawnersCommand : ModCommand
{
    public override string Command => "toggleNPCSpawners";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }
        var player = caller.Player.GetModPlayer<NPCSpawnerPlayer>();

        player.ShowAllSpawnPoints = !player.ShowAllSpawnPoints;

        Main.NewText("Showing all NPC spawn points: " + player.ShowAllSpawnPoints);
    }
}

public class CurrentNPCSpawnerCommand : ModCommand
{
    public override string Command => "currentNPCSpawners";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }
        var player = caller.Player.GetModPlayer<NPCSpawnerPlayer>();
        Main.NewText($"ID: {player.CurrentNPCType}");
        for (int i = 0; i < player.AdditionalData.Length; i++)
        {
            Main.NewText($"AdditionalData[{i}]: {player.CurrentNPCType}");
        }
    }
}

public class ClearEmptyNPCSpawnerCommand : ModCommand
{
    public override string Command => "clearEmptyNPCSpawners";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }

        NPCSpawnerSystem system = GetInstance<NPCSpawnerSystem>();
        foreach (var key in system.NPCSpawnPoints.Where(kvp => kvp.Value.id == -1)
                                                 .Select(kvp => kvp.Key)
                                                 .ToList())
        {
            system.NPCSpawnPoints.Remove(key);
        }

        Main.NewText("Cleared empty NPC spawners.");
    }
}

public class ActivateNPCSpawnersCommand : ModCommand
{
    public override string Command => "activateNPCSpawners";

    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }

        NPCSpawnerSystem.ActivateNPCSpawners();

        Main.NewText("Force activated NPC spawners.");
    }
}
