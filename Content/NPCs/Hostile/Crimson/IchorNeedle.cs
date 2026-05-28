using NeoParacosm.Content.Projectiles.Hostile.Evil;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class IchorNeedle : ModNPC
{
    int AITimer = 0;
    int AttackTimer = 0;

    ref float MoveSpeed => ref NPC.ai[0];
    ref float PlayerWhoAmI => ref NPC.ai[1];
    ref float OffsetX => ref NPC.ai[2];
    ref float OffsetY => ref NPC.ai[3];
    Vector2 hitOffset => new Vector2(OffsetX, OffsetY);
    Player hitPlayer => Main.player[((int)PlayerWhoAmI)];

    Vector2 targetPos;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 56;
        NPC.height = 100;
        NPC.lifeMax = 300;
        NPC.defense = 16;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 5000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.5f;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
        PlayerWhoAmI = -1;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(targetPos);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        targetPos = reader.ReadVector2();
    }

    public override void OnSpawn(IEntitySource source)
    {
        PlayerWhoAmI = -1;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        int planteraMul = NPC.downedPlantBoss ? 3 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMul;
        int planteraMulDF = NPC.downedPlantBoss ? 4 : 1;
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
        Dust.NewDustPerfect(NPC.Center, DustID.Crimson);
        //SoundEngine.PlaySound(SoundID.NPCDeath66 with { Volume = 0.2f }, NPC.Center);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        if (PlayerWhoAmI == -1)
        {
            PlayerWhoAmI = target.whoAmI;
            OffsetX = (target.Center - NPC.Center).X;
            OffsetY = (target.Center - NPC.Center).Y;
        }

        target.AddBuff(BuffID.Ichor, 60 * 60);
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        MoveSpeed *= 0.5f;
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        MoveSpeed *= 0.5f;
    }

    public override void AI()
    {
        if (NPC.velocity.Length() < 0.02f)
        {
            NPC.velocity = Vector2.UnitY * 0.1f;
        }
        NPC.TargetClosest(false);

        if (PlayerWhoAmI == -1)
        {
            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
            if (NPC.HasValidTarget)
            {
                Player player = Main.player[NPC.target];
                NPC.TurningMoveToPos(player.Center, 10, MoveSpeed);
                if (Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2))
                {
                    MoveSpeed += 0.02f;
                }
                else
                {
                    int attemptCount = 0;
                    //Dust.NewDustPerfect(targetPos, DustID.GemRuby, Scale: 2f).noGravity = true; ;
                    while (!Collision.CanHitLine(NPC.Center, 2, 2, targetPos, 2, 2) && attemptCount < 100)
                    {
                        if (LemonUtils.NotClient())
                        {
                            targetPos = player.Center + Main.rand.NextVector2Circular(300, 300);
                        }
                        attemptCount++;
                    }

                    NPC.netUpdate = true;

                    if (attemptCount >= 100)
                    {
                        NPC.TurningMoveToPos(player.Center, 10, MoveSpeed);

                    }
                    else
                    {
                        NPC.TurningMoveToPos(targetPos, 10, MoveSpeed);
                    }


                    MoveSpeed *= 0.99f;
                }
            }
            else
            {
                MoveSpeed *= 0.98f;
            }

            MoveSpeed = MathHelper.Clamp(MoveSpeed, 1, 10);
        }
        else
        {
            if (hitPlayer == null || !hitPlayer.IsAlive())
            {
                NPC.SimpleStrikeNPC(6699, 1);
                return;
            }

            NPC.Center = hitPlayer.Center - hitOffset;
            NPC.rotation = hitOffset.ToRotation() - MathHelper.PiOver2;
        }
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 15;
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
        int chanceBoost = NPC.downedPlantBoss ? 4 : 1;
        return (spawnInfo.Player.ZoneCrimson && Main.hardMode) ?
            0.02f * chanceBoost
            : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }
}
