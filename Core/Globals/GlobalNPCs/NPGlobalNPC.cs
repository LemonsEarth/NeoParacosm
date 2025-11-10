using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;

namespace NeoParacosm.Core.Globals.GlobalNPCs;

public class NPGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        if (spawnInfo.Player.InModBiome<DeadForestBiome>())
        {
            pool.Clear();
            if (!DownedBossSystem.downedDeathbird)
            {
                pool.Add(NPCType<Deathbird>(), 1f);
            }
        }
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (player.HasBuff<ReducedSpawns>())
        {
            spawnRate = (int)(spawnRate * 5f);
            maxSpawns = (int)(maxSpawns * 0.5f);
        }
    }
}
