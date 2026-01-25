using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.DeadForest;

public class StaffKnight : ModNPC
{
    int AITimer = 0;
    int castingCooldown = 0;
    int attackChoice = 1;
    int castingTimer = 0;
    bool casting = false;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(castingCooldown);
        writer.Write(attackChoice);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        castingCooldown = reader.ReadInt32();
        attackChoice = reader.ReadInt32();
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 8;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 72;
        NPC.height = 66;
        NPC.lifeMax = 500;
        NPC.defense = 25;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath55;
        NPC.value = 10000;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.2f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
        {
            //BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.,
        });
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 3; i++)
            {
                LemonUtils.SmokeGore(NPC.GetSource_FromAI(), NPC.RandomPos(), 2, 3);
            }
        }
        else
        {
            Dust.NewDustDirect(NPC.RandomPos(-16, -16), 2, 2, DustID.Stone);
        }
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {

    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {

    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {

    }

    public override bool PreAI()
    {
        NPC.TargetClosest(true);
        if (NPC.HasPlayerTarget)
        {
            NPC.DiscourageDespawn(600);
        }
        else
        {
            return true;
        }
        NPC.spriteDirection = -NPC.direction;
        Player player = Main.player[NPC.target];

        if (castingCooldown == 0 && !casting)
        {
            if (LemonUtils.NotClient())
            {
                castingCooldown = Main.rand.Next(180, 360);
            }
            NPC.netUpdate = true;
            casting = true;
        }

        if (casting)
        {
            if (castingTimer >= 180)
            {
                if (LemonUtils.NotClient())
                {
                    attackChoice = Main.rand.NextBool().ToDirectionInt();
                }
                NPC.netUpdate = true;
                if (attackChoice == -1)
                {
                    if (LemonUtils.NotClient())
                    {
                        for (int i = -2; i <= 2; i++)
                        {
                            LemonUtils.QuickProj(
                                NPC,
                                NPC.Center,
                                NPC.DirectionTo(player.Center).RotatedBy(i * MathHelper.Pi / 8) * 5,
                                ProjectileType<SmallHolyBlast>(),
                                NPC.damage / 4,
                                ai1: 1f,
                                ai2: 360f
                                );
                        }
                    }
                }
                else
                {
                    if (LemonUtils.NotClient())
                    {
                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            int healHP = (int)(npc.lifeMax * 0.33f);
                            if (npc.life + healHP > npc.lifeMax)
                            {
                                healHP = npc.lifeMax - npc.life;
                            }

                            npc.life += healHP;
                            npc.HealEffect(healHP);
                        }
                    }
                    NPC.netUpdate = true;
                }
                casting = false;
                castingTimer = 0;
            }
            NPC.velocity.X = 0;
            castingTimer++;
            return false;
        }
        else
        {
            if (castingCooldown > 0)
            {
                castingCooldown--;
            }
        }


        return true;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 20;
        if (casting)
        {
            frameDur = 6;
            NPC.frameCounter += 1;
            if (NPC.frameCounter > frameDur)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= 8 * frameHeight)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        else
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > frameDur)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= 4 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[NPC.type].Value;
        Rectangle sourceRect = NPC.frame;
        Vector2 drawOrigin = sourceRect.Size() * 0.5f;
        for (int k = NPC.oldPos.Length - 1; k >= 0; k--)
        {
            Vector2 drawPos = (NPC.oldPos[k] + drawOrigin - Main.screenPosition);
            Color color = k == 0 ? Color.White : Color.Black * 0.5f;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, NPC.rotation, drawOrigin, NPC.scale, LemonUtils.SpriteDirectionToSpriteEffects(-NPC.spriteDirection), 0);
        }
        return false;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return DownedBossSystem.downedDeathbird && spawnInfo.Player.InModBiome<DeadForestBiome>() ? 0.1f : 0f;

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
