using System.IO;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems.Data;

public class DownedBossSystem : ModSystem
{
    public static bool downedDeathbird = false;
    public static bool downedDeathbirdMini = false;
    public static bool downedDreadlord = false;
    public static bool downedDeathKnightCaptain = false;

    public override void ClearWorld()
    {
        downedDeathbird = false;
        downedDeathbirdMini = false;
        downedDreadlord = false;
        downedDeathKnightCaptain = false;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        downedDeathbird = tag.ContainsKey(nameof(downedDeathbird));
        downedDeathbirdMini = tag.ContainsKey(nameof(downedDeathbirdMini));
        downedDreadlord = tag.ContainsKey(nameof(downedDreadlord));
        downedDeathKnightCaptain = tag.ContainsKey(nameof(downedDeathKnightCaptain));
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (downedDeathbird)
        {
            tag[nameof(downedDeathbird)] = true;
        }

        if (downedDeathbird)
        {
            tag[nameof(downedDeathbirdMini)] = true;
        }

        if (downedDreadlord)
        {
            tag[nameof(downedDreadlord)] = true;
        }

        if (downedDeathKnightCaptain)
        {
            tag[nameof(downedDeathKnightCaptain)] = true;
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        BitsByte flags = new BitsByte();
        flags[0] = downedDeathbird;
        flags[1] = downedDeathbirdMini;
        flags[2] = downedDreadlord;
        flags[3] = downedDeathKnightCaptain;
        writer.Write(flags);
    }

    public override void NetReceive(BinaryReader reader)
    {
        BitsByte flags = reader.ReadByte();
        downedDeathbird = flags[0];
        downedDeathbirdMini = flags[1];
        downedDreadlord = flags[2];
        downedDeathKnightCaptain = flags[3];
    }
}
