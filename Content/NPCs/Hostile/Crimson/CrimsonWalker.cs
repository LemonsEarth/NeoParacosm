using Microsoft.Xna.Framework;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class CrimsonWalker : ModNPC
{
    float AITimer = 0;

    enum State
    {
        Idle,
        Lunge
    }

    State state = State.Idle;
    bool close = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 50;
        NPC.height = 50;
        NPC.lifeMax = 10;
        NPC.defense = 1;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 100;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.6f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
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

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {

    }

    public override void AI()
    {
        NPC.TargetClosest(true);
        NPC.spriteDirection = NPC.direction;

        if (!NPC.HasValidTarget)
        {
            AITimer++;
            return;
        }
        Player player = Main.player[NPC.target];

        if (Main.rand.NextBool(300))
        {
            SoundEngine.PlaySound(SoundID.Mummy with { PitchRange = (-1f, 1f) });
            SoundEngine.PlaySound(SoundID.ZombieMoan with { PitchRange = (-1f, 1f) });
        }

        if (NPC.Center.Distance(player.Center) < 300)
        {
            close = true;
        }
        else if (NPC.Center.Distance(player.Center) > 1000)
        {
            close = false;
            AITimer = 0;
        }

        if (close)
        {
            if (AITimer % 180 == 0 && AITimer > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.velocity += NPC.Center.DirectionTo(player.Center) * Main.rand.NextFloat(7, 15);
                }
                NPC.netUpdate = true;
            }
        }

        if (Math.Abs(NPC.velocity.X) < 1)
        {
            state = State.Idle;
        }
        else
        {
            state = State.Lunge;
        }

        NPC.velocity.X /= 1.05f;

        AITimer++;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        int count = Main.rand.Next(1, 4);
        for (int i = 0; i < count; i++)
        {
            Gore.NewGore(NPC.GetSource_FromThis(), NPC.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), ModContent.GoreType<RedGore>());
        }
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 20;
        NPC.frameCounter += 1;
        int startFrame = state == State.Lunge ? 0 : 1;
        int maxFrame = state == State.Lunge ? 0 : 2;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > maxFrame * frameHeight)
            {
                NPC.frame.Y = startFrame * frameHeight;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return (spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight) ? 0.1f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
