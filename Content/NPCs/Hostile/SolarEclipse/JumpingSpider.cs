using System.Collections.Generic;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.SolarEclipse;

public class JumpingSpider : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
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
            NPC.noTileCollide = true;
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
        //npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
