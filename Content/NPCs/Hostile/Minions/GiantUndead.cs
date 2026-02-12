using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;
using ReLogic.Content;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.Minions;

public class GiantUndead : ModNPC
{
    int AITimer = 0;
    ref float AttackInterval => ref NPC.ai[0];
    ref float AttackTimer => ref NPC.ai[1];
    ref float Attack => ref NPC.ai[2];
    ref float DeathbirdIndex => ref NPC.ai[3];
    Vector2 startPos = Vector2.Zero;
    Vector2 randomPos = Vector2.Zero;
    Vector2 randomPos2 = Vector2.Zero;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 2;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
    }

    static Asset<Texture2D> neckTexture;
    public override void Load()
    {
        neckTexture = Request<Texture2D>("NeoParacosm/Content/NPCs/Hostile/Minions/GiantUndeadNeck");
    }

    public override void SetDefaults()
    {
        NPC.width = 100;
        NPC.height = 100;
        NPC.lifeMax = 600;
        NPC.defense = 10;
        NPC.damage = 80;
        NPC.HitSound = SoundID.DD2_SkeletonDeath;
        NPC.DeathSound = SoundID.DD2_SkeletonSummoned;
        NPC.value = 100;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.5f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
    }

    public override bool CheckActive()
    {
        return false;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        NPC.lifeMax = (Main.expertMode ? 1000 : 600) * ((numPlayers / 2) + 1);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        /*bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });*/
    }

    public override void OnKill()
    {
        for (int i = 0; i < 16; i++)
        {
            Dust.NewDustPerfect(NPC.RandomPos(16, 16), DustID.Ash, Scale: 3f).noGravity = true;
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {

    }

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    public override void AI()
    {
        NPC.TargetClosest(true);
        if (!Main.npc[(int)DeathbirdIndex].active || Main.npc[(int)DeathbirdIndex].type != NPCType<Deathbird>())
        {
            for (int i = 0; i < 16; i++)
            {
                Dust.NewDustPerfect(NPC.RandomPos(16, 16), DustID.Ash, Scale: 3f).noGravity = true;
            }
            NPC.active = false;
        }
        if (AITimer == 0)
        {
            startPos = NPC.position;
            if (LemonUtils.NotClient())
            {
                AttackInterval = Main.rand.Next(150, 210);
            }
            NPC.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Zombie105 with { PitchRange = (0, 0.5f) }, NPC.Center);
        }
        Dust.NewDustDirect(startPos, 32, 32, DustID.Ash, Scale: 3f).noGravity = true;

        if (!NPC.HasValidTarget)
        {
            AITimer++;
            return;
        }

        Player player = Main.player[NPC.target];

        Vector2 dirToPlayer = NPC.DirectionTo(player.Center);
        if (AITimer % AttackInterval - 30 == 0)
        {
            if (LemonUtils.NotClient())
            {
                randomPos2 = startPos + startPos.DirectionTo(player.Center) * Main.rand.NextFloat(32, 128);
            }
            NPC.netUpdate = true;
        }

        if (randomPos2 != Vector2.Zero)
        {
            startPos = Vector2.Lerp(startPos, randomPos2, 1 / 30f);
        }
        NPC.spriteDirection = -NPC.direction;
        float angleCorrection = NPC.spriteDirection == 1 ? MathHelper.Pi : 0;
        NPC.rotation = dirToPlayer.ToRotation() + angleCorrection;

        if (AITimer % 180 == 0)
        {
            if (LemonUtils.NotClient())
            {
                randomPos = startPos + Main.rand.NextVector2CircularEdge(100, 100);
            }
            NPC.netUpdate = true;
        }
        if (randomPos != Vector2.Zero)
        {
            NPC.MoveToPos(randomPos, 0.2f, 0.2f, 0.2f, 0.2f);
        }
        else
        {
            NPC.MoveToPos(startPos, 0.2f, 0.2f, 0.1f, 0.1f);
        }

        if (NPC.Distance(player.Center) < 700 && AITimer > 120)
        {
            if (AttackTimer % AttackInterval == 0)
            {
                if (LemonUtils.NotClient())
                {
                    Attack = Main.rand.Next(0, 2);
                }
                NPC.netUpdate = true;
            }

            if (Attack == 0)
            {
                if (AttackTimer % (AttackInterval / 3) == 0)
                {
                    NPC.velocity = NPC.DirectionTo(player.Center) * 20;
                }
            }
            else if (Attack == 1)
            {
                if (AttackTimer % (AttackInterval - 60) == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, NPC.Center.DirectionTo(player.Center) * 10, ProjectileType<DeathflameBall>(), NPC.damage / 4, ai0: 9999, ai1: NPC.target);
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath6 with { PitchRange = (0, 0.5f) }, NPC.Center);
                }
            }
            AttackTimer++;
        }
        else
        {
            if (AITimer % 120 == 0)
            {
                if (Main.npc[(int)DeathbirdIndex].life <= Main.npc[(int)DeathbirdIndex].lifeMax * 0.6f)
                {
                    if (LemonUtils.NotClient())
                    {
                        Main.npc[(int)DeathbirdIndex].HealEffect((int)(Main.npc[(int)DeathbirdIndex].lifeMax * 0.02f));
                        Main.npc[(int)DeathbirdIndex].life += (int)(Main.npc[(int)DeathbirdIndex].lifeMax * 0.02f);
                    }
                }
                LemonUtils.DustLine(NPC.Center, Main.npc[(int)DeathbirdIndex].Center, DustID.Ash, 16, 3f);

            }
        }

        AITimer++;
    }

    float controlPointDirection = 0;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Texture2D neckTex = neckTexture.Value;

        Vector2 startToPos = startPos.DirectionTo(NPC.Center);
        Vector2 drawPos = startPos;
        float maxDistance = startPos.Distance(NPC.Center);
        float distanceLeft = maxDistance;
        float segmentCount = 0;
        int maxSegments = (int)(maxDistance / (neckTex.Size().Length() * 0.3f));
        int goalControlPointDirection = startToPos.Y <= 0 ? -1 : 1;
        controlPointDirection = MathHelper.Lerp(controlPointDirection, goalControlPointDirection * -NPC.spriteDirection, 1 / 20f);
        Vector2 bezierControlPoint = startPos + startToPos.RotatedBy(MathHelper.ToRadians(45 * controlPointDirection)) * (maxDistance / 2);
        while (segmentCount < maxSegments)
        {
            float segmentProgress = segmentCount / maxSegments;
            float nextSegmentProgress = (segmentCount + 1) / maxSegments;
            distanceLeft -= neckTex.Height;
            drawPos = LemonUtils.BezierCurve(startPos, NPC.Center, bezierControlPoint, segmentProgress);
            Vector2 nextPos = LemonUtils.BezierCurve(startPos, NPC.Center, bezierControlPoint, nextSegmentProgress);
            float rotation = drawPos.DirectionTo(nextPos).ToRotation() + MathHelper.PiOver2;
            float scale = Math.Clamp(segmentCount, 0, maxSegments / 3) / (maxSegments / 3);
            Main.EntitySpriteDraw(neckTex, drawPos - Main.screenPosition, null, Color.White, rotation, neckTex.Size() * 0.5f, scale, LemonUtils.SpriteDirectionToSpriteEffects(-NPC.spriteDirection));
            segmentCount++;
        }

        Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, texture.Frame(1, 2, 0, (int)Attack), Color.White, NPC.rotation, new Vector2(46, 60), NPC.scale, LemonUtils.SpriteDirectionToSpriteEffects(NPC.spriteDirection));
        return false;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = frameHeight * (int)Attack;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }
}
