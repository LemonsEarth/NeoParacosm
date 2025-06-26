using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Items.Accessories.Combat;
using NeoParacosm.Content.Projectiles.Hostile;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class CrimsonSentryForm : ModNPC
{
    float AITimer = 0;
    float AttackTimer = 0;
    float AttackCount = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 68;
        NPC.height = 72;
        NPC.lifeMax = 130;
        NPC.defense = 3;
        NPC.damage = 10;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 100;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 12;
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
        int chanceBoost = NPC.downedBoss2 ? 4 : 1;
        return (spawnInfo.Player.ZoneCrimson && Main.tile[spawnInfo.SpawnTileX + 1, spawnInfo.SpawnTileY].HasTile && Main.tile[spawnInfo.SpawnTileX - 1, spawnInfo.SpawnTileY].HasTile) ? 0.07f * chanceBoost : 0f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.4f);
        int planteraMulHP = NPC.downedPlantBoss ? 3 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMulHP;
        int planteraMulDF = NPC.downedPlantBoss ? 10 : 1;
        NPC.defense *= planteraMulDF;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        NPC.TargetClosest(false);

        if (!NPC.HasValidTarget)
        {
            AITimer++;
            return;
        }

        Player player = Main.player[NPC.target];
        if (Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2))
        {
            if (NPC.Center.Distance(player.Center) < 500)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer % 90 == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 dir = NPC.Center.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30))) * Main.rand.NextFloat(5, 10);
                        LemonUtils.QuickProj(NPC, NPC.RandomPos(), dir, ModContent.ProjectileType<CrimsonThorn>());
                    }
                }
            }
            else
            {
                if (AITimer % 120 == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dir = NPC.Center.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-8, 8))) * Main.rand.NextFloat(20, 25);
                        LemonUtils.QuickProj(NPC, NPC.RandomPos(), dir, ModContent.ProjectileType<CrimsonThorn>());
                    }
                }
            }
        }

        AITimer++;
    }

    public override void OnKill()
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
        npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CommensalPathogen>(), 50, 25));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemID.Ichor, 2, 1, 2));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
