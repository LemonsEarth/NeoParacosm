using Microsoft.Xna.Framework;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class CrimsonCatalyst : ModNPC
{
    float AITimer = 0;
    int choosePositionTimer = 0;

    HashSet<Vector2> storedPositions = new();

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.width = 44;
        NPC.height = 76;
        NPC.lifeMax = 100;
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
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(10))
            {
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<CrimsonInfectionForm>());
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
        if (choosePositionTimer == 600)
        {
            if (storedPositions.Count == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
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
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (var pos in storedPositions)
                    {
                        NPC.NewNPCDirect(NPC.GetSource_FromThis(), pos, Main.rand.NextFromList(NPCID.FaceMonster, NPCID.BloodCrawler, NPCID.Crimera, ModContent.NPCType<CrimsonCarrier>(), ModContent.NPCType<CrimsonWalker>()));
                    }
                }
                storedPositions.Clear();
            }
            choosePositionTimer = 0;
            NPC.netUpdate = true;
        }
        if (storedPositions.Count > 0)
        {
            foreach (var pos in storedPositions)
            {
                Dust.NewDustDirect(pos, 32, 2, DustID.Crimson, 0, -10).noGravity = true;
            }
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
        return (spawnInfo.Player.ZoneCrimson) ? 0.04f : 0f;
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
