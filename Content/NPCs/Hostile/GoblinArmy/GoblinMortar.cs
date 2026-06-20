using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.GoblinArmy;

public class GoblinMortar : ModNPC
{
    float AITimer = 0;

    bool hasTarget = false;


    int hasTargetTimer = 0;
    int GoblinType = -1;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(GoblinType);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        GoblinType = reader.ReadInt32();
    }

    enum GoblinFrame
    {
        Peon = 4,
        Warrior = 5,
        Thief = 6,
        Sorcerer = 7
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 8;
    }

    public override void SetDefaults()
    {
        NPC.width = 48;
        NPC.height = 54;
        NPC.lifeMax = 120;
        NPC.defense = 15;
        NPC.damage = 10;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.Item14;
        NPC.value = 1000;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        NPC.knockBackResist = 0f;

    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void FindFrame(int frameHeight)
    {
        if (!hasTarget)
        {
            int frameDur = 10;
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
        else
        {
            if (GoblinType != -1)
            {
                switch (GoblinType)
                {
                    case NPCID.GoblinPeon:
                        NPC.frame.Y = (int)GoblinFrame.Peon * frameHeight;
                        break;
                    case NPCID.GoblinWarrior:
                        NPC.frame.Y = (int)GoblinFrame.Warrior * frameHeight;
                        break;
                    case NPCID.GoblinThief:
                        NPC.frame.Y = (int)GoblinFrame.Thief * frameHeight;
                        break;
                    case NPCID.GoblinSorcerer:
                        NPC.frame.Y = (int)GoblinFrame.Sorcerer * frameHeight;
                        break;
                }

            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return Main.invasionType == InvasionID.GoblinArmy ? 0.15f : 0f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions.Goblins,
            });
    }

    public override void OnSpawn(IEntitySource source)
    {
        GoblinType = Main.rand.Next(26, 29 + 1);
    }

    public override bool PreAI()
    {
        NPC.TargetClosest(true);
        NPC.spriteDirection = NPC.direction;

        if (NPC.HasValidTarget && NPC.GetTarget().Distance(NPC.Center) < 500f && Collision.CanHitLine(NPC.Center, 2, 2, NPC.GetTarget().Center, 2, 2))
        {
            hasTarget = true;

            NPC.velocity.X = 0;
            //NPC.velocity.Y++;

            if (hasTargetTimer >= 300)
            {
                hasTargetTimer = 0;
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustPerfect(NPC.Center + Vector2.UnitX * Main.rand.NextFloat(-8f, 8f), DustID.Ash, -Vector2.UnitY * Main.rand.NextFloat(2f, 5f), Scale: Main.rand.NextFloat(1.5f, 2f)).noGravity = true;
                }
                if (LemonUtils.NotClient())
                {
                    var goblin = NPC.NewNPCDirect(
                        NPC.GetSource_FromAI(),
                        NPC.Center,
                        GoblinType,
                        NPC.whoAmI,
                        target: NPC.target
                        );

                    Vector2 targetPos = NPC.GetTarget().Center - Vector2.UnitY * Main.rand.NextFloat(150, 500);
                    float launchMultiplier = MathHelper.Clamp(NPC.Distance(NPC.GetTarget().Center) / 500f, 0f, 1f);
                    float launchForce = Main.rand.NextFloat(10, 15) * launchMultiplier;
                    goblin.velocity = NPC.DirectionTo(targetPos) * launchForce;

                    NetMessage.SendData(MessageID.SyncNPC, number: goblin.whoAmI);
                }
            }

            hasTargetTimer++;
            AITimer++;
            return false;
        }
        hasTarget = false;
        hasTargetTimer = 0;
        return true;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        if (NPC.velocity.Y < 0f)
        {
            NPC.velocity.Y = 0f;
        }
        AITimer++;
    }

    public override void OnKill()
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Wood, minimumDropped: 20, maximumDropped: 40));

    }

    public override bool? CanFallThroughPlatforms()
    {
        return false;
    }
}
