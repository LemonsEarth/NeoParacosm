using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Core.Systems.Assets;
using System.IO;
using Terraria.Audio;

namespace NeoParacosm.Content.NPCs.Hostile.Tremeyem;

public class DimmedWarlock : ModNPC
{
    int AITimer = 0;
    int shootingCooldownTimer = 0;
    int shootingCooldown = 0;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(shootingCooldownTimer);
        writer.Write(shootingCooldown);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        shootingCooldownTimer = reader.ReadInt32();
        shootingCooldown = reader.ReadInt32();
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 2;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 42;
        NPC.height = 48;
        NPC.lifeMax = 70;
        NPC.defense = 0;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 50;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.9f;
    }

    public override bool PreAI()
    {
        if (AITimer == 0)
        {
            if (LemonUtils.NotClient())
            {
                if (shootingCooldown == 0) shootingCooldown = Main.rand.Next(120, 180);
            }
            NPC.netUpdate = true;
        }
        NPC.TargetClosest();
        NPC.velocity.X *= NPC.knockBackResist;
        if (!NPC.HasValidTarget)
        {
            return true;
        }

        Player player = NPC.GetTarget();
        Vector2 toPlayer = player.Center - NPC.Center;
        NPC.spriteDirection = -LemonUtils.Sign(toPlayer.X, 1);

        bool distanceCheck = toPlayer.LengthSquared() < 500 * 500;
        bool canHitLine = Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2);

        if (canHitLine && distanceCheck)
        {
            if (shootingCooldownTimer == 0)
            {
                SoundEngine.PlaySound(SFX.WraithHit with { PitchRange = (0.5f, 0.7f), Volume = 0.3f }, NPC.Center);
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(
                        NPC,
                        NPC.Center,
                        toPlayer.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 1.5f),
                        ProjectileType<DeathflameBallSmall>(),
                        ai0: 360,
                        ai1: NPC.target
                        );
                }
                shootingCooldownTimer = shootingCooldown;
            }
        }

        if (shootingCooldownTimer > 0)
        {
            shootingCooldownTimer--;
        }

        return false;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 30;
        NPC.frameCounter++;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 1 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        shootingCooldownTimer = shootingCooldown;
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
