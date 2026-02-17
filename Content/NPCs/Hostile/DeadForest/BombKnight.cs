using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.DeadForest;

public class BombKnight : ModNPC
{
    int AITimer = 0;
    int throwWindUpDuration = 120;
    int throwingTimer = 0;
    int notThrowingTimer = 0;

    bool throwing = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 8;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 128;
        NPC.height = 156;
        NPC.lifeMax = 1200;
        NPC.defense = 25;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath55;
        NPC.value = 10000;
        NPC.aiStyle = NPCAIStyleID.Fighter;
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
        NPC.spriteDirection = NPC.direction;
        Player player = Main.player[NPC.target];
        int minDistanceToTarget = 600;
        if (NPC.Distance(player.Center) < minDistanceToTarget)
        {
            if (notThrowingTimer > 210 && !throwing)
            {
                throwing = true;
                throwingTimer = throwWindUpDuration;
            }
        }

        if (!throwing)
        {
            notThrowingTimer++;
        }
        else
        {
            notThrowingTimer = 0;
            if (throwingTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(
                        NPC,
                        NPC.Top,
                        NPC.DirectionTo(player.Center) * Main.rand.NextFloat(12, 20),
                        ProjectileType<DarkIncendiaryProjHostile>(),
                        NPC.damage / 4,
                        1f,
                        ai0: 180,
                        ai1: NPC.target,
                        ai2: 15f
                        );
                }
            }

            if (throwingTimer <= -16)
            {
                throwing = false;
            }

            if (throwingTimer > -16)
            {
                throwingTimer--;
            }
            NPC.velocity.X = 0;
            return false;
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
        if (throwing)
        {
            NPC.frameCounter++;
            if (throwingTimer > 60)
            {
                frameDur = 4;
                if (NPC.frameCounter > frameDur && NPC.frame.Y < 7 * frameHeight)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                }
            }
            else if (throwingTimer > 0)
            {
                NPC.frame.Y = 7 * frameHeight;
                NPC.frameCounter = 0;
            }
            else if (throwingTimer > -16)
            {
                frameDur = 4;
                if (NPC.frameCounter > frameDur && NPC.frame.Y > 4 * frameHeight)
                {
                    NPC.frame.Y -= frameHeight;
                    NPC.frameCounter = 0;
                }
            }
        }
        else
        {
            NPC.frameCounter++;
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
        npcLoot.Add(ItemDropRule.Common(ItemType<DarkIncendiary>(), 20, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
