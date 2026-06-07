using NeoParacosm.Content.Items.Accessories.Movement;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.WorldBuilding;

namespace NeoParacosm.Content.NPCs.Hostile.SolarEclipse;

public class JumpingSpider : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.DoesntDespawnToInactivityAndCountsNPCSlots[Type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = 88;
        NPC.height = 28;
        NPC.lifeMax = 500;
        NPC.defense = 10;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 2000;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.6f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.scale = Main.rand.NextFloat(0.8f, 1.5f);
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Eclipse,
            });
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        target.AddBuff(BuffID.Venom, 180);
    }

    public override bool PreAI()
    {
        NPC.spriteDirection = NPC.direction;
        return true;
    }

    public override void AI()
    {
        NPC.TargetClosest(true);
        if (AITimer == 0)
        {

        }

        if (NPC.HasValidTarget && NPC.collideY && AITimer % 300 == 0)
        {
            NPC.velocity += NPC.Center.DirectionTo(Main.player[NPC.target].Center) * 10 - Vector2.UnitY * 5;
            AITimer = 0;
            WorldUtils.Gen(
                NPC.Center.ToTileCoordinates(),
                new Shapes.Circle((int)(3 * NPC.scale)),
                new Actions.Custom((i, j, args) =>
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        WorldGen.PlaceTile(i, j, TileID.Cobweb);
                    }
                    return true;
                }
                ));
            NPC.noTileCollide = true;
        }

        if (NPC.CheckAndDespawn(2000, 1000))
        {
            return;
        }

        if (NPC.velocity.Y > 0)
        {
            NPC.noTileCollide = false;
        }

        if (AITimer < 300)
        {
            AITimer++;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        if (NPC.velocity.Y < 0)
        {
            NPC.frame.Y = frameHeight * 3;
        }
        else if (NPC.velocity.Y > 0)
        {
            NPC.frame.Y = frameHeight * 0;
        }
        else
        {
            int frameDur = 3;
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
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return Main.eclipse && NPC.downedPlantBoss ? 0.15f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.SpiderFang, minimumDropped: 1, maximumDropped: 3));
        npcLoot.Add(ItemDropRule.Common(ItemType<JumpingSpiderLeg>(), 20, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
