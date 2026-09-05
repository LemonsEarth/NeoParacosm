using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Placeable.Tiles.Depths;
using NeoParacosm.Core.Globals.GlobalNPCs.Evil;
using NeoParacosm.Core.Systems.Data;
using Terraria.GameContent.Bestiary;
using static Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

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
        if (DarkCataclysmSystem.DarkCataclysmActive)
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
                if (item == Biomes.TheCorruption ||
                    item == Biomes.UndergroundCorruption ||
                    item == Biomes.CorruptDesert ||
                    item == Biomes.CorruptUndergroundDesert ||
                    item == Biomes.CorruptIce ||
                    item == Biomes.TheCrimson ||
                    item == Biomes.UndergroundCrimson ||
                    item == Biomes.CrimsonDesert ||
                    item == Biomes.CrimsonUndergroundDesert ||
                    item == Biomes.CrimsonIce)
                {
                    if (!AdaptsToDamageTypeNPC.EvilEnemiesBonus.Contains(i))
                    {
                        AdaptsToDamageTypeNPC.EvilEnemies.Add(i);
                    }
                }
            }
        }
        AddedNPCsToEvilCollections = true;
    }
}
