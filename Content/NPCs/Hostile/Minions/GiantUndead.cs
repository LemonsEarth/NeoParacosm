using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.Accessories.Misc;
using NeoParacosm.Content.Projectiles.Hostile;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Minions;

public class GiantUndead : ModNPC
{
    ref float AITimer => ref NPC.ai[0];
    ref float AttackTimer => ref NPC.ai[1];
    Vector2 startPos = Vector2.Zero;
    Vector2 randomPos = Vector2.Zero;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
    }

    static Asset<Texture2D> neckTexture;
    public override void Load()
    {
        neckTexture = ModContent.Request<Texture2D>("NeoParacosm/Content/NPCs/Hostile/Minions/GiantUndeadNeck");
    }

    public override void SetDefaults()
    {
        NPC.width = 100;
        NPC.height = 100;
        NPC.lifeMax = 300;
        NPC.defense = 10;
        NPC.damage = 40;
        NPC.HitSound = SoundID.DD2_SkeletonDeath;
        NPC.DeathSound = SoundID.DD2_SkeletonSummoned;
        NPC.value = 100;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.5f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
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

        if (AITimer == 0)
        {
            startPos = NPC.position;
        }
        Dust.NewDustDirect(startPos, 32, 32, DustID.Ash, Scale: 3f).noGravity = true;

        if (!NPC.HasValidTarget)
        {
            AITimer++;
            return;
        }

        Player player = Main.player[NPC.target];

        if (AITimer % 180 == 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                randomPos = startPos + Main.rand.NextVector2CircularEdge(100, 100);
            }
            NPC.netUpdate = true;
        }

        if (randomPos != Vector2.Zero)
        {
            NPC.MoveToPos(randomPos, 0.05f, 0.05f, 0.3f, 0.3f);
        }
        else
        {
            NPC.MoveToPos(startPos, 0.1f, 0.1f, 0.2f, 0.2f);
        }

        Vector2 dirToPlayer = NPC.DirectionTo(player.Center);

        NPC.spriteDirection = -NPC.direction;
        float angleCorrection = NPC.spriteDirection == 1 ? MathHelper.Pi : 0;
        NPC.rotation = dirToPlayer.ToRotation() + angleCorrection;
        


        if (NPC.Distance(player.Center) < 700)
        {

        }
        else
        {

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
            Main.EntitySpriteDraw(neckTex, drawPos - Main.screenPosition, null, Color.White * segmentProgress, rotation, neckTex.Size() * 0.5f, scale, LemonUtils.SpriteDirectionToSpriteEffects(-NPC.spriteDirection));
            segmentCount++;
        }

        Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, LemonUtils.SpriteDirectionToSpriteEffects(NPC.spriteDirection));
        return false;
    }

    public override void FindFrame(int frameHeight)
    {

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
