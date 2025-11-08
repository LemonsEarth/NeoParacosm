using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.NPCs.Hostile.Special;
using NeoParacosm.Content.Tiles.Special;
using NeoParacosm.Core.Globals.GlobalNPCs.Evil;
using Terraria.DataStructures;

namespace NeoParacosm.Core.Globals.GlobalNPCs;

public class NPBuffNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    int timer = 0;

    public bool hitByRuneOfExtinction { get; set; } = false;

    public Point16 dataCollectorTEPos { get; set; } = Point16.Zero;
    public Point16 dataCollectorEXTEPos { get; set; } = Point16.Zero;

    public override void ResetEffects(NPC npc)
    {
        //dataCollectorTEPos = Point16.Zero;

    }

    public override void PostAI(NPC npc)
    {
        if (npc.HasBuff(BuffType<LightsBaneDebuff>()))
        {
            if (timer % 5 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.RandomPos(16, 16), Main.rand.NextVector2Unit(), ProjectileID.LightsBane, 30, 0f, Main.myPlayer, Main.rand.NextFloat(0.75f, 1.25f));
                }
            }
            timer++;
        }
        else if (npc.HasBuff(BuffType<LightsBaneDebuff2>()))
        {
            int attackInterval = 30;
            if (timer >= 45) attackInterval = 10;
            if (timer >= 75) attackInterval = 3;
            if (timer % attackInterval == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.RandomPos(16, 16), Main.rand.NextVector2Unit(), ProjectileID.LightsBane, 60, 0f, Main.myPlayer, Main.rand.NextFloat(0.75f, 1.25f));
                }
            }
            timer++;
        }
        else
        {
            timer = 0;
        }
    }

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (npc.HasBuff(BuffType<CrimsonRotDebuff>()))
        {
            modifiers.Defense.Flat -= (10 - (npc.life / npc.lifeMax * 10));
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (npc.HasBuff(BuffType<CrimsonRotDebuff>()))
        {
            DOTDebuff(npc, 24, ref damage);
        }

        if (npc.HasBuff(BuffType<DeathflameDebuff>()))
        {
            float damagePerSecond = npc.lifeMax * 0.005f + 10;
            if (damagePerSecond > 50) damagePerSecond = 50;
            DOTDebuff(npc, damagePerSecond, ref damage);
        }
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (player.HasBuff(BuffType<ProvokedPresenceDebuff>()) && !NPC.AnyNPCs(NPCType<Marauder>()))
        {
            spawnRate /= 3;
            maxSpawns *= 4;
        }
    }

    void DOTDebuff(NPC npc, float damagePerSecond, ref int damage)
    {
        if (npc.lifeRegen > 0) npc.lifeRegen = 0;
        npc.lifeRegen -= (int)(damagePerSecond * 2);
        if (damage < damagePerSecond)
        {
            damage = (int)damagePerSecond;
        }
    }

    bool avariceLootBonus = false;
    public override void OnKill(NPC npc)
    {
        if (npc.SpawnedFromStatue) return;

        if (Main.rand.NextBool(5) && !avariceLootBonus && npc.HasBuff(BuffType<SkullOfAvariceDebuff>()))
        {
            avariceLootBonus = true;
            for (int i = 0; i < 4; i++)
            {
                npc.NPCLoot();
            }
        }

        if (dataCollectorTEPos != Point16.Zero && TileEntity.TryGet<DataCollectorTileEntity>(dataCollectorTEPos, out DataCollectorTileEntity dataCollector))
        {
            if (EvilGlobalNPC.EvilEnemiesBonus.Contains(npc.type))
            {
                dataCollector.CollectData(3);
            }
            else if (EvilGlobalNPC.EvilEnemies.Contains(npc.type))
            {
                dataCollector.CollectData();
            }
        }

        if (dataCollectorEXTEPos != Point16.Zero && TileEntity.TryGet<DataCollectorEXTileEntity>(dataCollectorEXTEPos, out DataCollectorEXTileEntity dataCollectorEX))
        {
            if (EvilGlobalNPC.EvilEnemiesBonus.Contains(npc.type))
            {
                dataCollectorEX.CollectData(3);
            }
            else if (EvilGlobalNPC.EvilEnemies.Contains(npc.type))
            {
                dataCollectorEX.CollectData();
            }
        }
    }
}
