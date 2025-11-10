using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class CrimsonCarrier : ModNPC
{
    float AITimer = 0;

    bool spawnedEnemies = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 56;
        NPC.height = 74;
        NPC.lifeMax = 70;
        NPC.defense = 15;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 500;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.FaceMonster;
        NPC.knockBackResist = 0.3f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        int planteraMul = NPC.downedPlantBoss ? 10 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMul;
        int planteraMulDF = NPC.downedPlantBoss ? 3 : 1;
        NPC.defense *= planteraMulDF;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
    }

    public override void OnKill()
    {
        if (spawnedEnemies) return;

        spawnedEnemies = true;
        if (LemonUtils.NotClient())
        {
            NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, NPCType<CrimsonCarrierHead>());
        }
    }

    public override void AI()
    {
        NPC.spriteDirection = NPC.direction;
        if (Main.rand.NextBool(100))
        {
            Gore.NewGoreDirect(NPC.GetSource_FromThis("PeriodicSmoke"), NPC.RandomPos(), Main.rand.NextVector2Unit() * 2, GoreType<RedSmokeGore>());
        }
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 20;
        NPC.frameCounter += 1;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 2 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!Main.bloodMoon)
        {
            return (spawnInfo.Player.ZoneCrimson) ? 0.1f : 0f;
        }
        else
        {
            return (spawnInfo.Player.ZoneOverworldHeight) ? 0.15f : 0f;
        }

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
