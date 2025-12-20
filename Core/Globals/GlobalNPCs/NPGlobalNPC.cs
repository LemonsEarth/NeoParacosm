using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Core.Globals.GlobalNPCs.Evil;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Core.Globals.GlobalNPCs;

public class NPGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public Dictionary<DamageClass, float> DamageReductions { get; private set; } = new Dictionary<DamageClass, float>();

    public void SetDamageReductions(params (DamageClass damageType, float value)[] damageReductions)
    {
        foreach ((DamageClass damageType, float value) in damageReductions)
        {
            this.DamageReductions.Add(damageType, value);
        }
    }

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

    public override bool PreAI(NPC npc)
    {
        if (ResearcherQuest.DarkCataclysmActive)
        {
            if (npc.friendly || npc.CountsAsACritter || npc.realLife != -1 || npc.boss || npc.immortal || npc.lifeMax > 1000)
            {
                return true;
            }
            var combined = new HashSet<int>(EvilGlobalNPC.EvilEnemies);
            combined.UnionWith(EvilGlobalNPC.EvilEnemiesBonus);
            if (!combined.Contains(npc.type))
            {
                npc.active = false;
                return false;
            }
        }
        return true;
    }

    public override void SetStaticDefaults()
    {

    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (player.HasBuff<ReducedSpawns>())
        {
            spawnRate = (int)(spawnRate * 5f);
            maxSpawns = (int)(maxSpawns * 0.5f);
        }

        if (ResearcherQuest.DarkCataclysmActive)
        {
            spawnRate = (int)(spawnRate * 0.75f);
            maxSpawns = (int)(maxSpawns * 2f);
        }
    }

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (modifiers.DamageType != null && DamageReductions.TryGetValue(modifiers.DamageType, out float dr))
        {
            modifiers.FinalDamage *= (100 - dr) / 100f;
        }
    }
}
