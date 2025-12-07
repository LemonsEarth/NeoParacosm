using NeoParacosm.Content.Tiles.DeadForest;
using NeoParacosm.Content.Tiles.Depths;
using NeoParacosm.Core.Globals.GlobalNPCs.Evil;
using NeoParacosm.Core.Systems.Data;
using System.Linq;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Core.Systems.World;

public class BiomeSystem : ModSystem
{
    public int depthStoneTileCount = 0;
    public int deadDirtTileCount = 0;

    bool AddedNPCsToEvilCollections = false;

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        depthStoneTileCount = tileCounts[TileType<DepthStoneBlock>()];
        deadDirtTileCount = tileCounts[TileType<DeadDirtBlock>()];
    }

    public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
    {
        if (ResearcherQuest.Progress == ResearcherQuest.ProgressState.DownedResearcher)
        {
            tileColor = new Color(90 / 255f, 6 / 255f, 82 / 255f, 1);
        }
    }

    public override void OnWorldLoad()
    {
        if (AddedNPCsToEvilCollections) return;
        for (int i = 0; i < NPCLoader.NPCCount; i++)
        {
            BestiaryEntry entry = BestiaryDatabaseNPCsPopulator.FindEntryByNPCID(i);
            foreach (var item in entry.Info)
            {
                if (item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptDesert ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptUndergroundDesert ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptIce ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCrimson ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonDesert ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonUndergroundDesert ||
                    item == BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonIce)
                {
                    if (!EvilGlobalNPC.EvilEnemiesBonus.Contains(i))
                    {
                        EvilGlobalNPC.EvilEnemies.Add(i);
                    }
                }
            }
        }
        AddedNPCsToEvilCollections = true;
    }
}
