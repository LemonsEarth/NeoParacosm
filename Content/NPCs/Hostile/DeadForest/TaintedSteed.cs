using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.DeadForest;

public class TaintedSteed : ModNPC
{
    int AITimer = 0;
    ref float RandomEvent => ref NPC.ai[0];
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 84;
        NPC.height = 64;
        NPC.lifeMax = 500;
        NPC.defense = 25;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCDeath18;
        NPC.DeathSound = SoundID.NPCDeath18;
        NPC.value = 10000;
        NPC.aiStyle = NPCAIStyleID.Unicorn;
        //AIType = NPCID.DesertBeast;
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
        if (NPC.velocity.X > 2)
        {
            NPC.velocity.X = 2;
        }
        else if (NPC.velocity.X < -2)
        {
            NPC.velocity.X = -2;
        }

        if (AITimer % 365 == 0 && AITimer > 0)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(
                    NPC,
                    NPC.Center,
                    Vector2.Zero,
                    ProjectileType<SuckyProjectile>(),
                    0,
                    ai0: 1000,
                    ai1: 65,
                    ai2: 3
                    );
            }
            SoundEngine.PlaySound(SoundID.NPCDeath18 with { PitchRange = (0.25f, 0.5f)}, NPC.Center);
            SoundEngine.PlaySound(SoundID.Zombie54 with { PitchRange = (-0.5f, -0.25f)}, NPC.Center);
        }

        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 20;
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
