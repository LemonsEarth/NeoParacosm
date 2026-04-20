using System.IO;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems.Data;

public class TownNPCRespawnSystem : ModSystem
{
    public static bool UnlockedDragonWorshipperSpawn { get; set; } = false;

    public override void ClearWorld()
    {
        UnlockedDragonWorshipperSpawn = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag[nameof(UnlockedDragonWorshipperSpawn)] = UnlockedDragonWorshipperSpawn;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        UnlockedDragonWorshipperSpawn = tag.GetBool(nameof(UnlockedDragonWorshipperSpawn));
    }

    public override void NetSend(BinaryWriter writer)
    {
        BitsByte flags = new BitsByte();
        flags[0] = UnlockedDragonWorshipperSpawn;
        writer.Write(flags);
    }

    public override void NetReceive(BinaryReader reader)
    {
        BitsByte flags = reader.ReadByte();
        UnlockedDragonWorshipperSpawn = flags[0];
    }
}
