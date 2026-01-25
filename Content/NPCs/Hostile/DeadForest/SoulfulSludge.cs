using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.DeadForest;

public class SoulfulSludge : ModNPC
{
    int AITimer = 0;
    ref float Phase => ref NPC.ai[0];
    ref float AttackTimer => ref NPC.ai[1];
    float scaleX = 1f;
    float scaleY = 1f;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 50;
        NPC.lifeMax = 300;
        NPC.defense = 8;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.Item167;
        NPC.value = 5000;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.knockBackResist = 0f;
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

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {

    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {

    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {

    }

    public override bool PreAI()
    {
        NPC.TargetClosest(true);
        if (NPC.HasPlayerTarget)
        {
            NPC.DiscourageDespawn(600);
        }
        NPC.spriteDirection = -NPC.direction;


        return true;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 6; i++)
            {
                LemonUtils.SmokeGore(NPC.GetSource_FromAI(), NPC.RandomPos(), 0, 2.5f);
            }
            for (int i = 0; i < 3; i++)
            {
                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f), Main.rand.Next(41, 44));
            }
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.t_Slime, newColor: Color.Black, Scale: 2f);
            }
        }
        else
        {
            Dust.NewDustDirect(NPC.RandomPos(-8, -8), 2, 2, DustID.Stone);
            Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.t_Slime, newColor: Color.Black, Scale: 1f);
        }
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        // Alternatively, Phase = Floor((1 - NPC.GetLifePercent()) * 4)
        // But I think this is more readable
        switch (NPC.GetLifePercent())
        {
            case >= 0.75f:
                Phase = 0;
                break;
            case >= 0.5f:
                Phase = 1;
                break;
            case >= 0.25f:
                Phase = 2;
                break;
            case >= 0f:
                Phase = 3;
                break;
        }

        if (AttackTimer == 180 - Phase * 30)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickPulse(NPC, NPC.Center, 1f, 2f * (Phase + 1), 5, Color.DarkGray);
            }
            SoundEngine.PlaySound(SoundID.Mummy with { PitchRange = (-0.4f, 0f) }, NPC.Center);
            foreach (var player in Main.ActivePlayers)
            {
                if (player.Distance(NPC.Center) < 200 + 50 * Phase)
                {
                    player.AddBuff(BuffID.OgreSpit, (45 + (30 * (int)Phase + 1)) * LemonUtils.GetDifficulty());
                }
            }
        }
        else if (AttackTimer >= 360 - Phase * 30)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath13 with { PitchRange = (-0.6f, -0.4f) }, NPC.Center);
            if (LemonUtils.NotClient() && NPC.HasPlayerTarget)
            {
                for (int i = 0; i < 3 + Phase; i++)
                {
                    LemonUtils.QuickProj(
                        NPC,
                        NPC.RandomPos(),
                        Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(0.5f, 1f) * (Phase + 1),
                        ProjectileType<DeathflameBallSmall>(),
                        ai0: 60,
                        ai1: NPC.target
                        );
                }
            }
            AttackTimer = 0;
        }

        NPC.velocity.X *= 0.97f;
        AttackTimer++;
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = frameHeight * (int)Phase;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        // Sine waves go from (0.75 to 1.25)
        // 1 when 0
        scaleX = (MathF.Sin(AITimer / 24f) * 0.1f + 1) * NPC.scale;

        // 1.25 when 0
        scaleY = (MathF.Sin(AITimer / 24f + MathHelper.PiOver2) * 0.1f + 1) * NPC.scale;
        Texture2D texture = TextureAssets.Npc[NPC.type].Value;
        Rectangle sourceRect = NPC.frame;
        Vector2 drawOrigin = new Vector2(sourceRect.Width * 0.5f, sourceRect.Height);
        Main.EntitySpriteDraw(texture, NPC.position + drawOrigin - Main.screenPosition, sourceRect, drawColor, NPC.rotation, drawOrigin, new Vector2(scaleX, scaleY), LemonUtils.SpriteDirectionToSpriteEffects(-NPC.spriteDirection), 0);

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
