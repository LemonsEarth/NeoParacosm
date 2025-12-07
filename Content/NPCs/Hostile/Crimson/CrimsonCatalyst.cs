using NeoParacosm.Common.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class CrimsonCatalyst : ModNPC
{
    float AITimer = 0;
    int choosePositionTimer = 0;
    int spawnCount = 0;

    HashSet<Vector2> storedPositions = new();

    static HashSet<int> PreHM_Enemies = new HashSet<int>()
    {
        NPCID.FaceMonster, NPCID.BloodCrawler, NPCID.Crimera, NPCType<CrimsonCarrier>(), NPCType<CrimsonWalker>(), NPCType<RotPerfumeValve>()
    };

    static HashSet<int> HM_Enemies = new HashSet<int>()
    {
        NPCID.Crimslime, NPCID.Herpling, NPCID.CrimsonAxe, NPCID.IchorSticker, NPCID.FloatyGross, NPCType<CrimsonCarrier>(), NPCType<CrimsonWalker>(),NPCType<CrimsonSentryForm>(), NPCType<RotPerfumeValve>()
    };

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.width = 44;
        NPC.height = 76;
        NPC.lifeMax = 70;
        NPC.defense = 20;
        NPC.damage = 20;
        NPC.HitSound = SoundID.DD2_SkeletonDeath;
        NPC.DeathSound = SoundID.DD2_SkeletonSummoned;
        NPC.value = 1000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
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
        LemonUtils.DustCircle(NPC.Center, 8, 8, DustID.Crimson, 2.5f);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.expertMode)
        {
            if (LemonUtils.NotClient() && Main.rand.NextBool(10))
            {
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, NPCType<CrimsonInfectionForm>());
            }
        }
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(storedPositions.Count);
        foreach (Vector2 position in storedPositions)
        {
            writer.WriteVector2(position);
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        storedPositions.Clear();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            storedPositions.Add(reader.ReadVector2());
        }
    }

    public override void AI()
    {
        int timerCD = Main.hardMode ? 300 : 600;
        if (NPC.downedPlantBoss) timerCD = 180;
        if (choosePositionTimer == timerCD)
        {
            if (storedPositions.Count == 0)
            {
                if (LemonUtils.NotClient())
                {
                    for (int i = 0; i < LemonUtils.GetDifficulty() * 2; i++)
                    {
                        int attemptCount = 0;
                        while (attemptCount < 1000)
                        {
                            Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(300, 300);
                            if (Main.tile[(int)pos.X / 16, (int)pos.Y / 16].HasTile)
                            {
                                attemptCount++;
                                continue;
                            }
                            storedPositions.Add(pos);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (LemonUtils.NotClient())
                {
                    foreach (var pos in storedPositions)
                    {
                        List<int> enemiesToSpawn = Main.hardMode ? HM_Enemies.ToList() : PreHM_Enemies.ToList();
                        NPC.NewNPCDirect(NPC.GetSource_FromThis(), pos, Main.rand.NextFromCollection(enemiesToSpawn));
                    }
                }
                spawnCount++;
                storedPositions.Clear();
            }
            choosePositionTimer = 0;
            NPC.netUpdate = true;
        }
        if (storedPositions.Count > 0)
        {
            if (choosePositionTimer % 30 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item29 with { PitchRange = (-1f, -0.5f), Volume = 0.5f }, NPC.Center);
                LemonUtils.DustCircle(NPC.Center, 8, 8, DustID.GemRuby, choosePositionTimer / 100f);
            }
            foreach (var pos in storedPositions)
            {
                Dust.NewDustDirect(pos, 32, 2, DustID.Crimson, 0, -10, Scale: 1.5f).noGravity = true;
            }
        }

        if (spawnCount >= 3)
        {
            NPC.SimpleStrikeNPC(80085, 1, true);
        }
        Lighting.AddLight(NPC.Center, 3, 0, 0);
        choosePositionTimer++;
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
            if (NPC.frame.Y > 3 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        int chanceBoost = NPC.downedBoss2 ? 4 : 1;
        return (spawnInfo.Player.ZoneCrimson && !spawnInfo.Player.ZoneUnderworldHeight) ? 0.05f * chanceBoost : 0f;
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
