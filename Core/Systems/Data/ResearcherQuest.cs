using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using System.IO;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems.Data;

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
        DownedPlantera,
        DownedResearcher,
    }

    public static bool DarkCataclysmActive => Progress == ProgressState.DownedResearcher;

    public override void PostUpdateWorld()
    {
        if (Progress == ProgressState.NotDownedEvilBoss && NPC.downedBoss2)
        {
            Vector2 remainsPos = DragonRemainsTileEntityPos.ToWorldCoordinates();
            NPC.NewNPCDirect(new EntitySource_Misc("ResearcherEvilBossSpawn"), remainsPos + new Vector2(500, 0), NPCType<Researcher>());
            Progress = ProgressState.DownedEvilBoss;

            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.NeoParacosm.NPCs.Researcher.ResearcherAlert"), Color.Orange);
        }

        if (Main.hardMode && Progress == ProgressState.AscendedItem)
        {
            Progress = ProgressState.Hardmode;
        }

        if (Progress == ProgressState.CollectedData2 && NPC.downedMechBossAny)
        {
            Progress = ProgressState.DownedMechBoss;
        }

        if (Progress == ProgressState.DownedMechBoss && NPC.downedPlantBoss)
        {
            Progress = ProgressState.DownedPlantera;
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
            DragonRemainsTileEntityPos = tag.Get<Point16>("DragonRemainsTileEntityPos");
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
