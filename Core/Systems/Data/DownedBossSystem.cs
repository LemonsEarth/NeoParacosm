using System.IO;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems.Data;

public class DownedBossSystem : ModSystem
{
    public static bool downedDeathbird = false;
    public static bool downedDeathbirdMini = false;

    public override void ClearWorld()
    {
        downedDeathbird = false;
        downedDeathbirdMini = false;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        downedDeathbird = tag.ContainsKey(nameof(downedDeathbird));
        downedDeathbirdMini = tag.ContainsKey(nameof(downedDeathbirdMini));
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
    }

    public override void NetSend(BinaryWriter writer)
    {
        BitsByte flags = new BitsByte();
        flags[0] = downedDeathbird;
        flags[1] = downedDeathbirdMini;
        writer.Write(flags);
    }

    public override void NetReceive(BinaryReader reader)
    {
        BitsByte flags = reader.ReadByte();
        downedDeathbird = flags[0];
        downedDeathbirdMini = flags[1];
    }
}
