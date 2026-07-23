using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using System.IO;
using Terraria.Audio;

namespace NeoParacosm.Content.NPCs.Hostile.Tremeyem;

public class FacelessArcher : ModNPC
{
    int AITimer = 0;
    bool shooting = false;
    int shootingTimer = 0;
    int shootingPrepDuration = 0;
    int shootingShootDuration = 0;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(shootingPrepDuration);
        writer.Write(shootingShootDuration);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        shootingPrepDuration = reader.ReadInt32();
        shootingShootDuration = reader.ReadInt32();
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 6;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 56;
        NPC.lifeMax = 50;
        NPC.defense = 1;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 50;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.6f;
    }

    bool distanceCheck = false;
    bool canHitLine = false;
    public override bool PreAI()
    {
        if (AITimer == 0)
        {
            if (LemonUtils.NotClient())
            {
                if (shootingPrepDuration == 0) shootingPrepDuration = Main.rand.Next(75, 90);
                if (shootingShootDuration == 0) shootingShootDuration = Main.rand.Next(45, 60);
            }
            NPC.netUpdate = true;
        }
        NPC.TargetClosest();

        if (!NPC.HasValidTarget)
        {
            return true;
        }

        Player player = NPC.GetTarget();
        Vector2 toPlayer = player.Center - NPC.Center;

        distanceCheck = toPlayer.LengthSquared() < 400 * 400;
        canHitLine = Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2);

        if (canHitLine && distanceCheck)
        {
            shooting = true;
        }
        else if (canHitLine && !distanceCheck)
        {
            shooting = false;
            shootingTimer = 0;
            return true;
        }
        else
        {
            NPC.velocity.X = 0;
            shooting = false;
            shootingTimer = 0;
            return false;
        }

        if (shooting)
        {
            ShootingBehavior(player, toPlayer);
            AITimer++;
            return false;
        }
        else
        {
            shootingTimer = 0;
        }

        return false;
    }

    void ShootingBehavior(Player player, Vector2 toPlayer)
    {
        if (shootingTimer < shootingPrepDuration) // Prepare dash
        {
            if (shootingTimer == shootingPrepDuration / 2)
            {
                if (LemonUtils.NotClient() && Main.rand.NextBool(4))
                {
                    SoundEngine.PlaySound(SFX.BowShot with { PitchRange = (-2f, -1f), Volume = 0.5f }, NPC.Center);
                    NPC.velocity += -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 10;
                }
            }
            NPC.spriteDirection = -LemonUtils.Sign(toPlayer.X, 1);
            NPC.velocity *= 0.91f;
        }
        else if (shootingTimer == shootingPrepDuration) // Dash
        {
            NPC.velocity.X = 0;
            SoundEngine.PlaySound(SFX.BowShot with { PitchRange = (0.3f, 0.6f), Volume = 0.75f }, NPC.Center);
            NPC.spriteDirection = -LemonUtils.Sign(toPlayer.X, 1);

            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(
                    NPC,
                    NPC.Center,
                    toPlayer.SafeNormalize(Vector2.Zero) * 10,
                    ProjectileID.WoodenArrowHostile
                    );
            }
        }

        if (shootingTimer > shootingPrepDuration + shootingShootDuration)
        {
            NPC.knockBackResist = 0.6f;
            shooting = false;
            shootingTimer = 0;
        }

        shootingTimer++;
    }

    public override void AI()
    {
        if (NPC.HasValidTarget)
        {
            Vector2 toPlayer = NPC.GetTarget().Center - NPC.Center;
            NPC.spriteDirection = -LemonUtils.Sign(toPlayer.X, 1);
        }
        if (AITimer == 0)
        {

        }

        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int walkingMaxFrame = 3;
        int walkingFrameDuration = 18;
        int jumpingFrame = 3;
        int shootingFrame = 5;

        if (shooting)
        {
            NPC.frame.Y = shootingFrame * frameHeight;
        }
        else
        {
            if (NPC.collideY)
            {
                if (NPC.velocity.X != 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > walkingFrameDuration)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0;
                        if (NPC.frame.Y > walkingMaxFrame * frameHeight)
                        {
                            NPC.frame.Y = 0;
                        }
                    }
                }
                else
                {
                    NPC.frame.Y = 0;

                }
            }
            else
            {
                NPC.frame.Y = jumpingFrame * frameHeight;
            }
        }
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        shootingTimer = 0;
        if (NPC.life <= 0)
        {
            LemonUtils.DustBurst(10, NPC.Center, DustType<FireDust>(), 3, 3, 0.6f, 1f, Color.Black);
        }
        else
        {
            Dust.NewDustDirect(NPC.RandomPos(-8, -8), 2, 2, DustID.Stone);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return true;
    }

    public override bool CheckActive()
    {
        return false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemType<EclipseGreatshield>(), 10, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return false;
    }
}
