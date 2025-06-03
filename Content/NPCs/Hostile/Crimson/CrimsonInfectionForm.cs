using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class CrimsonInfectionForm : ModNPC
{
    float AITimer = 0;
    bool hitPlayer = false;
    int savedPlayerIndex = 0;
    Vector2 offset = Vector2.Zero;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 2;
    }

    public override void SetDefaults()
    {
        NPC.width = 24;
        NPC.height = 34;
        NPC.lifeMax = 10;
        NPC.defense = 1;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 100;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.6f;
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
        hitPlayer = true;
        savedPlayerIndex = target.whoAmI;
        offset = new Vector2(Main.rand.Next(0, Main.player[savedPlayerIndex].width), Main.rand.Next(0, Main.player[savedPlayerIndex].height));
    }

    public override bool PreAI()
    {
        if (!hitPlayer) return true;
        Player player = Main.player[savedPlayerIndex];
        if (player == null || !player.active || player.dead || player.ghost)
        {
            NPC.active = false;
            return false;
        }
        NPC.spriteDirection = NPC.direction;
        NPC.Center = player.position + offset;
        NPC.rotation = NPC.Center.DirectionTo(player.Center).ToRotation();
        return false;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.velocity = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloat(12, 18);
            }
            NPC.netUpdate = true;
        }
        NPC.spriteDirection = NPC.direction;

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
            if (NPC.frame.Y > 1 * frameHeight)
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
        //npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
