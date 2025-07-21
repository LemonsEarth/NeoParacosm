using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.NPCs.Hostile.Corruption;
using NeoParacosm.Content.NPCs.Hostile.Crimson;
using NeoParacosm.Content.NPCs.Hostile.Special;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;
using static Terraria.ID.NPCID;

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
                pool.Add(ModContent.NPCType<Deathbird>(), 1f);
            }
        }
    }
}
