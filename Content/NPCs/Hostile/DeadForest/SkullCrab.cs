using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Accessories.Combat.Defensive;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.DeadForest;

public class SkullCrab : ModNPC
{
    int AITimer = 0;

    bool alive = true;
    bool shouldDie = true;
    int deadTimer = 0;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 10;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 68;
        NPC.height = 60;
        NPC.lifeMax = 240;
        NPC.defense = 10;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit38 with { PitchRange = (-0.8f, -0.7f) };
        NPC.DeathSound = SoundID.NPCDeath41 with { PitchRange = (-0.8f, -0.7f) };
        NPC.value = 100;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.8f;
        SpawnModBiomes = [DeadForestBiome.BiomeID];
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(shouldDie);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        shouldDie = reader.ReadBoolean();
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
        if (AITimer == 0)
        {
            if (LemonUtils.NotClient() && Main.rand.NextBool(4))
            {
                shouldDie = false;
            }
            NPC.netUpdate = true;
        }

        if (!alive)
        {
            NPC.velocity *= 0.93f;
            deadTimer++;
            Dust.NewDustPerfect(
                NPC.Center + new Vector2(Main.rand.NextFloat(-NPC.width / 3, NPC.width / 3), NPC.height * 0.25f),
                DustType<FireDust>(),
                Vector2.UnitY * Main.rand.NextFloat(0.4f, 2f),
                newColor: Color.Black,
                Scale: Main.rand.NextFloat(0.3f, 0.6f)
                ).noGravity = true;
            if (deadTimer >= 180 && LemonUtils.NotClient())
            {
                int projAmount = LemonUtils.GetDifficulty() * 3;
                for (int i = 0; i < projAmount; i++)
                {
                    if (LemonUtils.NotClient())
                    {
                        float randomAngle = MathHelper.ToRadians(Main.rand.Next(-15, 15));
                        Vector2 velocity = Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / projAmount)).RotatedBy(randomAngle) * Main.rand.NextFloat(10, 17);
                        LemonUtils.QuickProj(NPC, NPC.Center, velocity, ProjectileType<DeathflameBall>(), ai0: 999, ai1: NPC.target);
                        LemonUtils.QuickProj(NPC, NPC.Center, Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(6, 10), ProjectileType<LingeringDeathflame>(), ai0: -1, ai1: 300);
                    }

                }
                NPC.StrikeInstantKill();
            }
            return false;
        }

        NPC.TargetClosest(true);
        if (NPC.HasPlayerTarget)
        {
            NPC.DiscourageDespawn(600);
        }
        NPC.spriteDirection = NPC.direction;


        return true;
    }

    public override bool CheckDead()
    {
        if (shouldDie) return true;
        alive = false;
        NPC.dontTakeDamage = true;
        NPC.life = NPC.lifeMax;
        return deadTimer > 180;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return !shouldDie && alive;
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
            Dust.NewDustDirect(NPC.RandomPos(-8, -8), 2, 2, DustID.Stone);
        }
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
        if (alive)
        {
            int frameDur = 6;
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
            if (NPC.frame.Y < 4 * frameHeight)
            {
                NPC.frame.Y = 4 * frameHeight;
                NPC.frameCounter = 0;
            }
            int frameDur = 12;
            NPC.frameCounter += 1;
            if (NPC.frameCounter > frameDur && NPC.frame.Y < 9 * frameHeight)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[NPC.type].Value;
        Vector2 drawOrigin = NPC.frame.Size() * 0.5f;
        Vector2 drawPos = NPC.Center;

        Main.EntitySpriteDraw(
            texture,
            drawPos - screenPos,
            NPC.frame,
            drawColor,
            NPC.rotation,
            drawOrigin,
            NPC.scale,
            LemonUtils.SpriteDirectionToSpriteEffects(NPC.spriteDirection)
            );
        return false;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return DownedBossSystem.downedDeathbirdMini && spawnInfo.Player.InModBiome<DeadForestBiome>() ? 0.02f : 0f;

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
