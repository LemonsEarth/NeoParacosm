using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.DeadForest;

public class WingedEyeball : ModNPC
{
    int AITimer = 0;
    bool dashing = false;
    int dashTimer = 0;
    int noDashTimer = 0;
    ref float targetPosX => ref NPC.ai[2];
    ref float targetPosY => ref NPC.ai[3];
    Vector2 targetPos => new Vector2(targetPosX, targetPosY);

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 7;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 64;
        NPC.lifeMax = 180;
        NPC.defense = 4;
        NPC.damage = 30;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.noGravity = true;
        NPC.value = 1200;
        NPC.noTileCollide = true;
        NPC.aiStyle = -1;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.5f;
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

        return true;
    }

    public override void AI()
    {
        NPC.TargetClosest(false);
        if (NPC.HasPlayerTarget)
        {
            NPC.DiscourageDespawn(600);
        }
        else
        {
            return;
        }
        NPC.spriteDirection = -NPC.direction;
        Player player = Main.player[NPC.target];

        if (AITimer == 0)
        {

        }

        if (!dashing)
        {
            NormalBehavior(player);
        }
        else
        {
            DashingBehavior(player);
        }

        AITimer++;
    }

    void NormalBehavior(Player player)
    {
        dashTimer = 0;
        NPC.rotation = 0;
        if (noDashTimer % 300 == 0)
        {
            if (LemonUtils.NotClient())
            {
                targetPosX = player.Center.X;
                targetPosY = player.Center.Y;
                int count = 100;
                do
                {
                    targetPosX = player.Center.X + Main.rand.NextFloat(-400, 400);
                    targetPosY = player.Center.Y + Main.rand.NextFloat(-400, 400);
                    count--;
                }
                while (Main.tile[(int)(targetPosX / 16), (int)(targetPosY / 16)].HasTile && count > 0);
            }
            NPC.netUpdate = true;
        }

        if (NPC.Center.Distance(targetPos) < 32)
        {
            NPC.velocity = Vector2.Zero;
            if (noDashTimer % 60 == 0)
            {
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(
                        NPC,
                        NPC.Center,
                        NPC.DirectionTo(player.Center) * 8,
                        ProjectileType<TearProjectile>(),
                        ai2: 180
                        );
                }
            }
        }
        else
        {
            NPC.velocity = NPC.DirectionTo(targetPos) * 6;
        }

        if (noDashTimer >= 600)
        {
            dashing = true;
        }

        noDashTimer++;
    }

    void DashingBehavior(Player player)
    {
        if (dashTimer == 0)
        {
            SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { PitchRange = (0.2f, 0.5f)}, NPC.Center);
            NPC.frame.Y = 4 * 64;
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustDirect(NPC.RandomPos(0, -16), 2, 2, DustID.GemDiamond, -NPC.velocity.X * 2, -NPC.velocity.Y * 2).noGravity = true;
            }
        }
        noDashTimer = 0;
        NPC.rotation = NPC.velocity.ToRotation();
        NPC.MoveToPos(player.Center, 0.05f, 0.05f, 0.2f, 0.2f);
        if (dashTimer >= 60)
        {
            dashing = false;
        }
        dashTimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 6;
        if (dashing)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > frameDur)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y > 6 * frameHeight)
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
                if (NPC.frame.Y > 3 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        /*Texture2D texture = TextureAssets.Npc[NPC.type].Value;
        Rectangle sourceRect = NPC.frame;
        Vector2 drawOrigin = sourceRect.Size() * 0.5f;
        for (int k = NPC.oldPos.Length - 1; k >= 0; k--)
        {
            Vector2 drawPos = (NPC.oldPos[k] + drawOrigin - Main.screenPosition);
            Color color = k == 0 ? Color.White : Color.Black * 0.5f;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, NPC.rotation, drawOrigin, NPC.scale, LemonUtils.SpriteDirectionToSpriteEffects(-NPC.spriteDirection), 0);
        }*/
        return true;
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
        return true;
    }
}
