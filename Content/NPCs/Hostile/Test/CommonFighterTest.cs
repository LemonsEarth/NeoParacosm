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
        NPC.width = 24;
        NPC.height = 48;
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

    public enum State
    {
        Patrolling,
        Investigating,
        Chasing
    }

    public bool CanSeePlayer(Player player)
    {
        return Collision.CanHit(NPC, player);
    }

    public bool CanSeeTarget()
    {
        Player player = NPC.GetTarget();
        return Collision.CanHit(NPC, player) && NPC.DistanceSQ(player.Center) < MaxTargetDistance * MaxTargetDistance;
    }

    public bool IsGrounded()
    {
        if (NPC.velocity.Y < 0) return false;
        Vector2 NPCBottom = new Vector2(NPC.Bottom.X, NPC.Bottom.Y + 8);
        Point NPCBottomPoint = NPCBottom.ToTileCoordinates();
        return NPCBottomPoint.HasSolidTile();
    }

    public bool NextStepHasTile()
    {
        Vector2 NPCBottom = new Vector2(NPC.Bottom.X, NPC.Bottom.Y + 8);
        Point NPCBottomPoint = NPCBottom.ToTileCoordinates();
        NPCBottomPoint.X += 1;
        return NPCBottomPoint.HasSolidTile();
    }

    public float MovementSpeed { get; set; } = 5f;
    public float ChaseSpeed { get; set; } = 8f;
    public float Acceleration { get; set; } = 0.1f;
    public float JumpHeight { get; set; } = 80f;
    public float AggroRange { get; set; } = 400f;
    public float MaxTargetDistance { get; set; } = 800f;

    public State CurrentState { get; set; } = State.Patrolling;
    public Vector2 OriginalHomePosition { get; set; }
    public Vector2 HomePosition { get; set; }
    public int CantSeeTargetTimer { get; set; } = 0;
    public Vector2 LastSeenTargetPos { get; set; }
    public Vector2 TargetPos { get; set; }
    public Vector2 DirToTargetPos => NPC.Center.DirectionTo(LastSeenTargetPos);

    public bool CanJump { get; set; } = false;

    public int PatrolTimer { get; set; } = 0;
    public int PatrolDistance { get; set; } = 160;
    public float MaxHomePositionDistance { get; set; } = 160;
    public float TooFarFromHomeTimer { get; set; } = 0;
    public float SwitchHomePositionInterval { get; set; } = 300;
    public int SwitchPatrolPositionInterval { get; set; } = 180;
    public int CantReachPatrolPositionTimer { get; set; } = 0;
    public Vector2 CurrentPatrolPos { get; set; }

    public bool DoTeleportToHome { get; set; }

    public int InvestigatingTimer { get; set; } = 0;
    public int InvestigatingDuration { get; set; } = 300;

    public void TryFindTarget()
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
            if (aggroAdjustedDistanceSQ < AggroRange * AggroRange)
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

        bool canSeeTarget = CanSeeTarget();
        if (!canSeeTarget)
        {
            CantSeeTargetTimer++;
        }
        else
        {
            LastSeenTargetPos = target.Center;
            CantSeeTargetTimer = 0;
        }

        if (CantSeeTargetTimer > InvestigatingDuration)
        {
            NPC.target = -1;
            CantSeeTargetTimer = 0;
            return;
        }
    }

    public void SetCurrentState()
    {
        Player target = NPC.GetTarget();
        if (target != null)
        {
            if (CanSeeTarget())
            {
                CurrentState = State.Chasing;
            }
            else
            {
                CurrentState = State.Investigating;
            }
        }
        else
        {
            CurrentState = State.Patrolling;
        }
    }

    public void StateControl()
    {
        switch (CurrentState)
        {
            case State.Patrolling:
                PatrollingBehavior();
                break;
            case State.Chasing:
                ChasingBehavior();
                break;
            case State.Investigating:
                InvestigatingBehavior();
                break;
        }
    }

    public bool IsPatrolPositionValid(ref Vector2 position)
    {
        Point posTileCoords = position.ToTileCoordinates();
        if (!WorldGen.InWorld(posTileCoords.X, posTileCoords.Y))
        {
            return false;
        }

        if (!posTileCoords.HasSolidTile())
        {
            for (int y = 1; y < 7; y++)
            {
                Point belowPos = new Point(posTileCoords.X, posTileCoords.Y + y);
                if (!WorldGen.InWorld(belowPos.X, belowPos.Y))
                {
                    return false;
                }
                if (belowPos.HasSolidTile())
                {
                    position = belowPos.ToWorldCoordinates();
                    return true;
                }
            }
            return false;
        }

        for (int y = 1; y < 7; y++)
        {
            Point abovePos = new Point(posTileCoords.X, posTileCoords.Y - y);
            if (!WorldGen.InWorld(abovePos.X, abovePos.Y))
            {
                return false;
            }
            if (!abovePos.HasSolidTile())
            {
                position = abovePos.ToWorldCoordinates();
                return true;
            }
        }
        return false;
    }

    public void PatrollingBehavior()
    {
        Dust.NewDustPerfect(CurrentPatrolPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
        if (DoTeleportToHome)
        {
            if (NPC.DistanceSQ(HomePosition) < 40 * 40)
            {
                DoTeleportToHome = false;
            }
            NPC.Opacity -= 1 / 60f;
            if (NPC.Opacity <= 0f)
            {
                NPC.Center = HomePosition;
                NPC.Opacity = 1f;
                LemonUtils.DustBurst(10, NPC.Center, DustID.GemDiamond, 5, 5, 1.2f, 2f);
            }
        }
        InvestigatingTimer = 0;
        if (PatrolTimer >= SwitchPatrolPositionInterval || AITimer == 0)
        {
            Vector2 patrolPos = HomePosition + new Vector2(Main.rand.NextFloat(-PatrolDistance, PatrolDistance), 0);
            if (IsPatrolPositionValid(ref patrolPos))
            {
                CurrentPatrolPos = patrolPos;
                PatrolTimer = 0;
            }
            else
            {
                NPC.velocity = Vector2.Zero;
            }
        }

        if (MathF.Abs(NPC.Center.X - CurrentPatrolPos.X) > 16)
        {
            if (!Collision.CanHitLine(NPC.Center, 2, 2, CurrentPatrolPos, 2, 2))
            {
                if (CantReachPatrolPositionTimer >= 300)
                {
                    DoTeleportToHome = true;
                    CantReachPatrolPositionTimer = 0;

                }
                CantReachPatrolPositionTimer++;
            }
            MoveToPos(CurrentPatrolPos, MovementSpeed);
            FacePosition(CurrentPatrolPos);
        }
        else
        {
            CantReachPatrolPositionTimer = 0;
            NPC.velocity.X = 0f;
        }

        if (PatrolTimer < SwitchPatrolPositionInterval)
        {
            PatrolTimer++;
        }
    }

    public void ChasingBehavior()
    {
        Dust.NewDustPerfect(LastSeenTargetPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
        CantReachPatrolPositionTimer = 0;
        InvestigatingTimer = 0;
        MoveToPos(LastSeenTargetPos, MovementSpeed);
        FacePosition(LastSeenTargetPos);

        if (LastSeenTargetPos.Y < NPC.Top.Y && IsGrounded() && !NextStepHasTile())
        {
            NPC.velocity.Y = -JumpHeight;
        }

        if (LastSeenTargetPos.Y < NPC.Top.Y && IsGrounded() && MathF.Abs(LastSeenTargetPos.X - NPC.Center.X) < JumpHeight * JumpHeight)
        {
            NPC.velocity.Y = -JumpHeight;
        }
    }

    public void InvestigatingBehavior()
    {
        Dust.NewDustPerfect(LastSeenTargetPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
        CantReachPatrolPositionTimer = 0;
        MoveToPos(LastSeenTargetPos, MovementSpeed);
        FacePosition(LastSeenTargetPos);

        if (LastSeenTargetPos.Y < NPC.Top.Y && IsGrounded() && !NextStepHasTile())
        {
            NPC.velocity.Y = -JumpHeight;
        }

        if (InvestigatingTimer < InvestigatingDuration)
        {
            InvestigatingTimer++;
        }
    }

    public void FacePosition(Vector2 pos)
    {
        float toPosition = pos.X - NPC.Center.X;
        NPC.spriteDirection = -LemonUtils.Sign(toPosition, 1);
    }

    public void MoveToPos(Vector2 position, float speed)
    {
        float dirX = LemonUtils.Sign(NPC.Center.DirectionTo(position).X, 1);
        if (dirX == 1 && NPC.velocity.X < speed)
        {
            NPC.velocity.X += dirX * Acceleration;
        }
        else if (dirX == -1 && NPC.velocity.X > -speed)
        {
            NPC.velocity.X += dirX * Acceleration;
        }

        if (LemonUtils.Sign(NPC.velocity.X, 1) != dirX)
        {
            NPC.velocity *= 0.9f;
        }

        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        int tileHeight = NPC.height / 16;
        for (int j = -tileHeight / 2; j <= tileHeight / 2; j++)
        {
            int xVelSign = LemonUtils.Sign(NPC.velocity.X, 1);
            Vector2 NPCFrontPosition = NPC.Center + new Vector2(xVelSign * NPC.width, 0);
            Point npcCenterTilePoint = NPCFrontPosition.ToTileCoordinates();
            npcCenterTilePoint.Y += j;

            Point tilePointInFront = npcCenterTilePoint;
            //LemonUtils.DustPoint(tilePointInFront);
            if (tilePointInFront.HasSolidTile() && IsGrounded())
            {
                NPC.velocity.Y = -JumpHeight;
                break;
            }
        }
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            OriginalHomePosition = NPC.Center;
            HomePosition = NPC.Center;
            CurrentPatrolPos = HomePosition;
            MovementSpeed = 2f;
            Acceleration = 0.05f;
            JumpHeight = 6f;
        }

        if (NPC.target == -1)
        {
            TryFindTarget();
        }

        CheckTarget();
        SetCurrentState();

        if (IsGrounded())
        {
            CanJump = true;
        }

        StateControl();
        //Main.NewText(CurrentState);
        //Main.NewText(NPC.target);

        /*for (int i = 0; i < 32; i++)
        {
            Dust.NewDustPerfect(NPC.Center - Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 32f + MathHelper.ToRadians(AITimer)) * 400, DustID.GemRuby, Vector2.Zero).noGravity = true;
        }*/

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
