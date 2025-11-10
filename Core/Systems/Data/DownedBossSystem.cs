using System.IO;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems.Data;

public class DownedBossSystem : ModSystem
{
    public static bool downedDeathbird = false;

    public override void ClearWorld()
    {
        downedDeathbird = false;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        downedDeathbird = tag.ContainsKey(nameof(downedDeathbird));
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (downedDeathbird)
        {
            tag[nameof(downedDeathbird)] = true;
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        BitsByte flags = new BitsByte();
        flags[0] = downedDeathbird;
        writer.Write(flags);
    }

    public override void NetReceive(BinaryReader reader)
    {
        BitsByte flags = reader.ReadByte();
        downedDeathbird = flags[0];
    }
}
