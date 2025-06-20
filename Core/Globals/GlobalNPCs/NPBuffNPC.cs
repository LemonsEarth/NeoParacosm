﻿using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using NeoParacosm.Content.Tiles.Special;
using Terraria.DataStructures;

namespace NeoParacosm.Core.Globals.GlobalNPCs;

public class NPBuffNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public Point16 dataCollectorTEPos { get; set; } = Point16.Zero;

    public override void ResetEffects(NPC npc)
    {
        //dataCollectorTEPos = Point16.Zero;
    }

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (npc.HasBuff(ModContent.BuffType<CrimsonRotDebuff>()))
        {
            modifiers.Defense.Flat -= (10 - (npc.life / npc.lifeMax * 10));
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (npc.HasBuff(ModContent.BuffType<CrimsonRotDebuff>()))
        {
            DOTDebuff(npc, 24, ref damage);
        }
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (player.HasBuff(ModContent.BuffType<ProvokedPresenceDebuff>()))
        {
            spawnRate /= 2;
            maxSpawns *= 3;
        }
    }

    void DOTDebuff(NPC npc, int damagePerSecond, ref int damage)
    {
        if (npc.lifeRegen > 0) npc.lifeRegen = 0;
        npc.lifeRegen -= damagePerSecond * 2;
        if (damage < damagePerSecond)
        {
            damage = damagePerSecond;
        }
    }


    public override void OnKill(NPC npc)
    {
        if (!npc.SpawnedFromStatue && dataCollectorTEPos != Point16.Zero
            && TileEntity.TryGet<DataCollectorTileEntity>(dataCollectorTEPos, out DataCollectorTileEntity dataCollector))
        {
            if (NPCLists.EvilEnemiesBonus.Contains(npc.type))
            {
                dataCollector.CollectData(3);
            }
            else if (NPCLists.EvilEnemies.Contains(npc.type))
            {
                dataCollector.CollectData();
            }
        }
    }
}
