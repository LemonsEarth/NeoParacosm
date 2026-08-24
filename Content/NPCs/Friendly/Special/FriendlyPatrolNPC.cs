using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoParacosm.Content.NPCs.Friendly.Special;

public abstract class FriendlyPatrolNPC : ModNPC
{
    public int AITimer = 0;
    ref float TimeLeft => ref NPC.ai[0];
    ref float DespawnDistance => ref NPC.ai[1];

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;
    }

    public override void SetDefaults()
    {
        NPC.friendly = true;
        NPCCooldowns = new Dictionary<NPC, int>();
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

    public NPC GetTarget()
    {
        if (TargetNPC == -1) return null;
        return Main.npc[TargetNPC];
    }

    public bool CanSeeNPC(NPC npc)
    {
        return Collision.CanHit(NPC, npc);
    }

    public bool CanSeeTarget()
    {
        NPC npc = GetTarget();
        return Collision.CanHit(NPC, npc) && NPC.DistanceSQ(npc.Center) < MaxTargetDistance * MaxTargetDistance;
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

    public int TargetNPC { get; set; } = -1;
    public float MovementSpeed { get; set; } = 2f;
    public float ChaseSpeed { get; set; } = 8f;
    public float Acceleration { get; set; } = 0.05f;
    public float JumpHeight { get; set; } = 6f;
    public float FallThroughBottomMargin { get; set; } = 8f;
    public float AggroRange { get; set; } = 400f;
    public float MaxTargetDistance { get; set; } = 800f;

    public void SetMovementValues(float movementSpeed = 2f, float chaseSpeed = 8f, float acceleration = 0.05f, float jumpHeight = 6f, float fallThroughBottomMargin = 8f)
    {
        MovementSpeed = movementSpeed;
        ChaseSpeed = chaseSpeed;
        Acceleration = acceleration;
        JumpHeight = jumpHeight;
        FallThroughBottomMargin = fallThroughBottomMargin;
    }

    public void SetAggroValues(float aggroRange = 400f, float maxTargetDistance = 800f)
    {
        AggroRange = aggroRange;
        MaxTargetDistance = maxTargetDistance;
    }

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

    public void SetPatrolValues(
        int patrolDistance = 160,
        float maxHomePositionDistance = 160,
        float switchHomePositionInterval = 300,
        int switchPatrolPositionInterval = 180)
    {
        PatrolDistance = patrolDistance;
        MaxHomePositionDistance = maxHomePositionDistance;
        SwitchHomePositionInterval = switchHomePositionInterval;
        SwitchPatrolPositionInterval = switchPatrolPositionInterval;
    }

    public bool DoTeleportToHome { get; set; }

    public int InvestigatingTimer { get; set; } = 0;
    public int InvestigatingDuration { get; set; } = 300;

    public void SetInvestigatingValues(int investigatingDuration = 300)
    {
        InvestigatingDuration = investigatingDuration;
    }

    public void TryFindTarget()
    {
        foreach (var npc in Main.ActiveNPCs)
        {
            if (!npc.CanBeChasedBy())
            {
                continue;
            }
            float distanceSQ = npc.DistanceSQ(NPC.Center);
            float aggroAdjustedDistanceSQ = distanceSQ;
            if (aggroAdjustedDistanceSQ < AggroRange * AggroRange)
            {
                bool canHitNPC = CanSeeNPC(npc);
                if (!canHitNPC) continue;
                TargetNPC = npc.whoAmI;
            }
        }
    }

    public void CheckTarget()
    {
        NPC target = GetTarget();
        if (target == null)
        {
            return;
        }

        if (!target.IsAlive())
        {
            TargetNPC = -1;
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
            TargetNPC = -1;
            CantSeeTargetTimer = 0;
            return;
        }
    }

    public void SetCurrentState()
    {
        NPC target = GetTarget();
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
        //Dust.NewDustPerfect(CurrentPatrolPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
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
        //Dust.NewDustPerfect(LastSeenTargetPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
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
        //Dust.NewDustPerfect(LastSeenTargetPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
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

    public virtual void Despawn()
    {
        NPC.active = false;
    }

    public void HandleDespawn()
    {
        if (TimeLeft > 0 && AITimer > TimeLeft)
        {
            Despawn();
            return;
        }
    }

    public void HandleHurtingNPCs()
    {
        foreach (var victim in Main.ActiveNPCs)
        {
            if (!victim.CanBeChasedBy())
            {
                continue;
            }

            if (victim.Hitbox.IntersectsExact(NPC.Hitbox))
            {
                HurtNPC(victim);
            }
        }
    }

    public float Knockback { get; set; } = 5;
    public int NPCHitCooldown { get; set; } = 60;
    public int BossHitCooldown { get; set; } = 60;
    public void SetCombatValues(float knockback = 5, int npcHitCooldown = 60, int bossHitCooldown = 60)
    {
        Knockback = 5;
        NPCHitCooldown = npcHitCooldown;
        BossHitCooldown = bossHitCooldown;
    }

    public int HurtNPC(NPC victim)
    {
        if (NPCCooldowns.ContainsKey(victim))
        {
            return 0;
        }
        int dir = LemonUtils.Sign(victim.Center.X - NPC.Center.X, 1);
        int cdValue = victim.boss ? BossHitCooldown : NPCHitCooldown;
        NPCCooldowns.Add(victim, cdValue);
        return victim.SimpleStrikeNPC(NPC.damage, dir, false, Knockback, damageVariation: false, noPlayerInteraction: true);
    }

    public Dictionary<NPC, int> NPCCooldowns = new Dictionary<NPC, int>();
    public void HandleNPCImmunityFrames()
    {
        foreach (var npc in NPCCooldowns.Keys)
        {
            NPCCooldowns[npc]--;
        }

        // Remove expired cooldowns
        foreach (var kvp in NPCCooldowns.ToList())
        {
            if (kvp.Value <= 0)
            {
                NPCCooldowns.Remove(kvp.Key);
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

        HandleDespawn();

        if (TargetNPC == -1)
        {
            TryFindTarget();
        }

        HandleHurtingNPCs();
        HandleNPCImmunityFrames();

        CheckTarget();
        SetCurrentState();

        if (IsGrounded())
        {
            CanJump = true;
        }

        StateControl();
        AITimer++;
    }

    public int WalkingStartFrame { get; set; } = 3;
    public int WalkingMaxFrame { get; set; } = 4;
    public int WalkingFrameDuration { get; set; } = 12;
    public int JumpingFrame { get; set; } = 1;
    public int FallingFrame { get; set; } = 2;
    public int IdleFrame { get; set; } = 0;

    public void SetAnimationValues(
        int walkingStartFrame = 3,
        int walkingMaxFrame = 4,
        int walkingFrameDuration = 12,
        int jumpingFrame = 1,
        int fallingFrame = 2,
        int idleFrame = 0
        )
    {
        WalkingStartFrame = walkingStartFrame;
        WalkingMaxFrame = walkingMaxFrame;
        WalkingFrameDuration = walkingFrameDuration;
        JumpingFrame = jumpingFrame;
        FallingFrame = fallingFrame;
        IdleFrame = idleFrame;
    }

    public override void FindFrame(int frameHeight)
    {
        //Main.NewText(NPC.velocity);

        if (NPC.collideY)
        {
            if (NPC.velocity.X != 0)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > WalkingFrameDuration)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                    if (NPC.frame.Y > WalkingMaxFrame * frameHeight)
                    {
                        NPC.frame.Y = WalkingStartFrame * frameHeight;
                    }
                    if (NPC.frame.Y < WalkingStartFrame * frameHeight)
                    {
                        NPC.frame.Y = WalkingStartFrame * frameHeight;
                    }
                }
            }
            else
            {
                NPC.frame.Y = IdleFrame * frameHeight;
            }
        }
        else
        {
            if (NPC.velocity.Y < 0)
            {
                NPC.frame.Y = JumpingFrame * frameHeight;
            }
            else
            {
                NPC.frame.Y = FallingFrame * frameHeight;
            }
        }

    }

    public override bool CheckActive()
    {
        return false;
    }

    public override bool? CanFallThroughPlatforms()
    {
        return NPC.ShouldFallThroughPlatforms(FallThroughBottomMargin);
    }
}
