using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems;

/// <summary>
/// Contains data related to the Researcher Quest
/// Researcher NPC, Dialogue and Ascension UI can be found in their respective files
/// </summary>
public class ResearcherQuest : ModSystem
{
    public static Point16 DragonRemainsTileEntityPos = Point16.Zero;
    public static ProgressState Progress = 0;
    public enum ProgressState
    {
        NotDownedEvilBoss, // NPC doesn't exist yet, Dragon Remains has a barrier
        DownedEvilBoss, // NPC spawned, Dragon Remains barrier disappears
        CollectedData, // Data Collector
        TalkedAfterCollectingData, // Unlocks evil weapon ascension
        AscendedItem,
        Hardmode,
        CollectedData2, // Data Collector EX
        DownedMechBoss,
        DownedPlantera
    }


    public override void PostUpdateWorld()
    {
        if (Progress == ProgressState.NotDownedEvilBoss && NPC.downedBoss2)
        {
            Vector2 remainsPos = DragonRemainsTileEntityPos.ToWorldCoordinates();
            NPC.NewNPCDirect(new EntitySource_Misc("ResearcherEvilBossSpawn"), remainsPos + new Vector2(500, 0), NPCType<Researcher>());
            Progress = ProgressState.DownedEvilBoss;
        }

        if (Progress == ProgressState.CollectedData2 && NPC.downedMechBossAny)
        {
            Progress = ProgressState.DownedMechBoss;
        }

        if (Main.hardMode && Progress == ProgressState.AscendedItem)
        {
            Progress = ProgressState.Hardmode;
        }
    }

    public override void ClearWorld()
    {
        Progress = ProgressState.NotDownedEvilBoss;
        DragonRemainsTileEntityPos = Point16.Zero;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        Progress = (ProgressState)tag.GetInt("ResearcherQuestProgress");
        if (tag.ContainsKey("DragonRemainsTileEntityPos"))
        {
            tag["DragonRemainsTileEntityPos"] = DragonRemainsTileEntityPos;
        }
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["ResearcherQuestProgress"] = (int)Progress;
        if (DragonRemainsTileEntityPos != Point16.Zero)
        {
            tag["DragonRemainsTileEntityPos"] = DragonRemainsTileEntityPos;
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write((int)Progress);
    }

    public override void NetReceive(BinaryReader reader)
    {
        reader.ReadInt32();
    }
}
