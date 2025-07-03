using System.IO;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems;

public class WorldDataSystem : ModSystem
{
    public static ResearcherQuestProgressState ResearcherQuestProgress = 0;
    public enum ResearcherQuestProgressState
    {
        NotDownedEvilBoss,
        DownedEvilBoss,
        CollectedData,
        TalkedAfterCollectingData,
        AscendedItem,
        Hardmode,
        CollectedData2,
    }

    public override void PostUpdateWorld()
    {
        if (Main.hardMode && ResearcherQuestProgress == ResearcherQuestProgressState.AscendedItem)
        {
            ResearcherQuestProgress = ResearcherQuestProgressState.Hardmode;
        }
    }

    public override void ClearWorld()
    {
        ResearcherQuestProgress = ResearcherQuestProgressState.NotDownedEvilBoss;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        ResearcherQuestProgress = (ResearcherQuestProgressState)tag.GetInt("ResearcherQuestProgress");
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["ResearcherQuestProgress"] = (int)ResearcherQuestProgress;
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write((int)ResearcherQuestProgress);
    }

    public override void NetReceive(BinaryReader reader)
    {
        reader.ReadInt32();
    }
}
