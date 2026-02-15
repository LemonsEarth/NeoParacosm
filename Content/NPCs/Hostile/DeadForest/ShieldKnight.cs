using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Items.Accessories.Combat;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.DeadForest;

public class ShieldKnight : ModNPC
{
    int AITimer = 0;
    int blockTimer = 0;
    int notBlockTimer = 0;
    int MAX_BLOCK_DURATION = 120;

    bool blocking = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 38;
        NPC.height = 58;
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

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {

    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (blocking) // if blocking, nullify damage and reduce penetrate, more in OnHitByProjectile
        {
            if (projectile.active)
            {
                projectile.penetrate--;
                modifiers.FinalDamage *= 0;
            }
        }
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (blocking)
        {
            // Fire projectile at closest player if blocking, keep blocking every time when hit by projectile
            Player closestPlayer = LemonUtils.GetClosestPlayer(NPC.Center);
            blockTimer = MAX_BLOCK_DURATION;
            LemonUtils.QuickProj(
                NPC,
                NPC.Center,
                NPC.DirectionTo(closestPlayer.Center) * 10,
                ProjectileType<HolyBlast>(),
                (hit.SourceDamage + NPC.damage) / 4,
                1f,
                ai1: 1.01f,
                ai2: 300
                );
            if (LemonUtils.IsHard())
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    LemonUtils.QuickProj(
                        NPC,
                        NPC.Center,
                        NPC.DirectionTo(closestPlayer.Center).RotatedBy(i * MathHelper.PiOver4) * 10,
                        ProjectileType<HolyBlast>(),
                        (hit.SourceDamage + NPC.damage) / 4,
                        1f,
                        ai1: 1.01f,
                        ai2: 300
                        );
                }
            }
        }
        else
        {
            // only block if hasn't blocked in 5 seconds with a 25% chance
            if (notBlockTimer > 300 && Main.rand.NextBool(4))
            {
                StartBlocking();
            }
        }
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

        if (blockTimer > 0)
        {
            blocking = true;
            notBlockTimer = 0;
            blockTimer--;
        }
        else
        {
            blocking = false;
            notBlockTimer++;
        }

        if ((notBlockTimer == 10 || notBlockTimer % 300 == 0) && NPC.HasPlayerTarget) // dash right when coming out of block
        {
            NPC.velocity = NPC.DirectionTo(Main.player[NPC.target].Center) * 10;
        }

        if (blocking)
        {
            NPC.noGravity = true;
            NPC.velocity = Vector2.Zero;
            return false;
        }
        NPC.noGravity = false;
        return true;
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

    void StartBlocking()
    {
        blockTimer = MAX_BLOCK_DURATION;
        if (LemonUtils.NotClient())
        {
            LemonUtils.QuickPulse(NPC, NPC.Center, 2f, 3f, 5f, Color.Gold);
        }
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 9;
        if (blocking)
        {
            NPC.frame.Y = 4 * frameHeight;
            NPC.frameCounter = 0;
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
        npcLoot.Add(ItemDropRule.Common(ItemType<EclipseGreatshield>(), 20, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
