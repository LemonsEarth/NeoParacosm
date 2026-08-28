using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.Systems.Particles;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.DeadForest;

public class DarkSkull : ModNPC
{
    int AITimer = 0;

    float actualSpeed = 0f;
    ref float TargetSpeed => ref NPC.ai[0];
    ref float StartDeathTimer => ref NPC.ai[1];
    int deathTimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 64;
        NPC.lifeMax = 100;
        NPC.defense = 2;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCDeath18;
        NPC.DeathSound = SoundID.NPCDeath18;
        NPC.value = 1000;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.7f;
        SpawnModBiomes = [DeadForestBiome.BiomeID];
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

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        target.AddBuff(BuffType<DeathflameDebuff>(), 120);
    }

    public override void OnKill()
    {
        LemonUtils.QuickProj(
            NPC,
            NPC.Center,
            Vector2.Zero,
            ProjectileType<DeathflameExplosion>(),
            ai0: 24,
            ai1: 1.5f + 0.5f * LemonUtils.GetDifficulty()
            );
    }

    public override bool PreAI()
    {
        return true;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            LemonUtils.ParticleBurst(
                8,
                NPC.Center,
                ParticleID.Gas,
                6f, 6f,
                0.6f, 0.9f,
                Color.Black
                );
        }
        else
        {
            Dust.NewDustDirect(NPC.RandomPos(-8, -8), 2, 2, DustID.Stone);
        }
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            if (LemonUtils.NotClient())
            {
                TargetSpeed = Main.rand.NextFloat(2, 4);
            }
            NPC.netUpdate = true;
        }

        NPC.TargetClosest();

        if (StartDeathTimer == 1)
        {
            deathTimer++;
            if (deathTimer >= 60)
            {
                NPC.SimpleStrikeNPC(NPC.lifeMax, 1, false, 0f, noPlayerInteraction: true);
            }
        }

        if (NPC.HasValidTarget)
        {
            Player player = NPC.GetTarget();
            if (NPC.DistanceSQ(player.Center) < 180 * 180 && StartDeathTimer == 0)
            {
                LemonUtils.ParticleBurst(
                    12,
                    NPC.Center,
                    ParticleID.Gas,
                    5f, 5f,
                    0.6f, 0.9f,
                    Color.Black
                );
                SoundEngine.PlaySound(SFX.ManaCrystal with { Volume = 0.7f, PitchRange = (-1f, -0.6f) });
                StartDeathTimer = 1;
                actualSpeed *= 0.3f;
                TargetSpeed *= 0.3f;
            }

            if (actualSpeed < TargetSpeed)
            {
                actualSpeed += 0.1f;
            }

            Vector2 dirToPlayer = NPC.DirectionTo(player.Center);

            NPC.velocity = dirToPlayer * actualSpeed;
            NPC.rotation = NPC.velocity.ToRotation();

            if (dirToPlayer.X < 0)
            {
                NPC.spriteDirection = -1;
                NPC.rotation += MathHelper.Pi;
            }
            else
            {
                NPC.spriteDirection = 1;
            }
        }

        ParticleSystem.SpawnParticle(
            ParticleID.Gas,
            NPC.RandomPos(),
            -Vector2.UnitY * Main.rand.NextFloat(1f, 3f),
            Color.Black,
            scale: Main.rand.NextFloat(0.2f, 0.6f)
            );
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 9;
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
        return DownedBossSystem.downedDeathbirdMini && Main.hardMode && spawnInfo.Player.InModBiome<DeadForestBiome>() ? 0.2f : 0f;

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
