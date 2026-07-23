using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using System.IO;

namespace NeoParacosm.Content.NPCs.Hostile.Test;

public class CommonFighterTest : ModNPC
{
    int AITimer = 0;

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 7;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 56;
        NPC.lifeMax = 100;
        NPC.defense = 3;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 50;
        //NPC.aiStyle = NPCAIStyleID.Fighter;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.6f;
    }

    public override bool PreAI()
    {

        return true;
    }

    public bool CanSeePlayer(Player player)
    {
        return Collision.CanHit(NPC, player);
    }

    public bool CanSeeTarget(Player player, float maxTargetDistance = 800)
    {
        return Collision.CanHit(NPC, player) && NPC.DistanceSQ(player.Center) < maxTargetDistance * maxTargetDistance;
    }

    int cantSeeTargetTimer = 0;
    Vector2 lastSeenTargetPos;

    public void TryFindTarget(float range)
    {
        foreach (var player in Main.ActivePlayers)
        {
            //Main.NewText(player.name);
            if (!player.IsAlive() || player.npcTypeNoAggro[Type])
            {
                continue;
            }
            float distanceSQ = player.DistanceSQ(NPC.Center);
            float aggroAdjustedDistanceSQ = distanceSQ + -player.aggro;
            if (aggroAdjustedDistanceSQ < range * range)
            {
                bool canHitPlayer = CanSeePlayer(player);
                if (!canHitPlayer) continue;
                NPC.target = player.whoAmI;
            }
        }
    }

    public void CheckTarget()
    {
        Player target = NPC.GetTarget();
        if (target == null)
        {
            return;
        }

        if (!target.IsAlive())
        {
            NPC.target = -1;
            return;
        }

        bool canSeeTarget = CanSeeTarget(target);
        if (!canSeeTarget)
        {
            cantSeeTargetTimer++;
        }
        else
        {
            lastSeenTargetPos = target.Center;
            cantSeeTargetTimer = 0;
        }

        if (cantSeeTargetTimer > 360)
        {
            NPC.target = -1;
            cantSeeTargetTimer = 0;
            return;
        }
    }

    public override void AI()
    {
        if (NPC.target == -1)
        {
            TryFindTarget(400);
        }

        CheckTarget();
        Main.NewText(NPC.target);
        Dust.NewDustPerfect(lastSeenTargetPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
        for (int i = 0; i < 32; i++)
        {
            Dust.NewDustPerfect(NPC.Center - Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 32f + MathHelper.ToRadians(AITimer)) * 400, DustID.GemRuby, Vector2.Zero).noGravity = true;
        }

        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int walkingMaxFrame = 3;
        int walkingFrameDuration = 18;
        int jumpingFrame = 3;

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


    public override void HitEffect(NPC.HitInfo hit)
    {
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
        return NPC.ShouldFallThroughPlatforms(8);
    }
}
