using NeoParacosm.Core.Systems.Expeditions;
using SubworldLibrary;
using System.Collections.Generic;
using System.Linq;

namespace NeoParacosm.Core.Commands;

public class CalculateNPCSpawnsCommand : ModCommand
{
    public override string Command => "calcspawns";
    public override bool IsCaseSensitive => false;
    public override CommandType Type => CommandType.World;
    public static bool counting = false;

    public static Dictionary<int, int> npcSpawnCounts = new Dictionary<int, int>();

    public override void Load()
    {
        On_NPC.NewNPC += On_NPC_NewNPC;
    }

    private int On_NPC_NewNPC(On_NPC.orig_NewNPC orig, Terraria.DataStructures.IEntitySource source, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target)
    {
        if (!counting) return orig(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
        if (npcSpawnCounts.TryGetValue(Type, out int count))
        {
            npcSpawnCounts[Type] = count + 1;
        }
        else
        {
            npcSpawnCounts[Type] = 1;
        }
        var result = orig(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
        Main.npc[result].active = false;
        return result;
    }

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (!ModLoader.TryGetMod("DragonLens", out Mod DragonLens))
        {
            Main.NewText("DragonLens must be enabled for this command!");
            return;
        }

        npcSpawnCounts.Clear();
        counting = true;
        for (int i = 0; i < 100000; i++)
        {
            NPC.SpawnNPC();
        }
        string msg = "NPC Spawn Counts:\n";
        foreach (var kvp in npcSpawnCounts)
        {
            string npcName = Lang.GetNPCNameValue(kvp.Key);
            float chance = (float)kvp.Value / npcSpawnCounts.Values.Sum();
            msg += $"{npcName}; Amount: {kvp.Value}; Chance: {chance:F2}\n";
        }
        counting = false;
        Mod.Logger.Info(msg);
    }
}
