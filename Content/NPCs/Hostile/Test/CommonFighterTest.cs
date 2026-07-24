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
        return Framing.GetTileSafely(NPCBottomPoint).HasTile;
    }

    public float MovementSpeed { get; set; } = 5f;
    public float Acceleration { get; set; } = 0.1f;
    public float JumpHeight { get; set; } = 80f;
    public float AggroRange { get; set; } = 400f;
    public float MaxTargetDistance { get; set; } = 800f;

    public State CurrentState { get; set; } = State.Patrolling;
    public Vector2 HomePosition { get; set; }
    public int CantSeeTargetTimer { get; set; } = 0;
    public Vector2 LastSeenTargetPos { get; set; }
    public Vector2 TargetPos { get; set; }
    public Vector2 DirToTargetPos => NPC.Center.DirectionTo(LastSeenTargetPos);

    public int PatrolTimer { get; set; } = 0;
    public int PatrolDistance { get; set; } = 160;
    public int SwitchPatrolPositionInterval { get; set; } = 180;
    public Vector2 CurrentPatrolPos { get; set; }

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

        if (CantSeeTargetTimer > 360)
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
        }
    }

    public bool IsPatrolPositionValid(ref Vector2 position)
    {
        Point posTileCoords = position.ToTileCoordinates();
        if (!WorldGen.InWorld(posTileCoords.X, posTileCoords.Y))
        {
            return false;
        }
        Tile posTile = Framing.GetTileSafely(posTileCoords.X, posTileCoords.Y);
        if (!posTile.HasTile)
        {
            for (int y = 1; y < 7; y++)
            {
                if (!WorldGen.InWorld(posTileCoords.X, posTileCoords.Y + y))
                {
                    return false;
                }
                Tile belowPosTile = Framing.GetTileSafely(posTileCoords.X, posTileCoords.Y + y);
                if (belowPosTile.HasTile)
                {
                    position = new Vector2(posTileCoords.X, posTileCoords.Y + y).ToWorldCoordinates();
                    return true;
                }
            }
            return false;
        }

        for (int y = 1; y < 7; y++)
        {
            if (!WorldGen.InWorld(posTileCoords.X, posTileCoords.Y - y))
            {
                return false;
            }
            Tile belowPosTile = Framing.GetTileSafely(posTileCoords.X, posTileCoords.Y - y);
            if (!belowPosTile.HasTile)
            {
                position = new Vector2(posTileCoords.X, posTileCoords.Y - y).ToWorldCoordinates();
                return true;
            }
        }
        return false;
    }

    public void PatrollingBehavior()
    {
        if (PatrolTimer >= SwitchPatrolPositionInterval || AITimer == 0)
        {
            Vector2 patrolPos = HomePosition + new Vector2(Main.rand.NextFloat(-PatrolDistance, PatrolDistance), 0);
            if (IsPatrolPositionValid(ref patrolPos))
            {
                CurrentPatrolPos = patrolPos;
                PatrolTimer = 0;
            }
        }

        if (NPC.DistanceSQ(CurrentPatrolPos) > 40 * 40)
        {
            MoveToPos(CurrentPatrolPos);
            float stepSpeed = 1f;
            float gfxOffY = 0;

            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            
            NPC.spriteDirection = -LemonUtils.Sign(NPC.velocity.X, 1);
        }
        else
        {
            NPC.velocity.X = 0f;
        }

        if (PatrolTimer < SwitchPatrolPositionInterval)
        {
            PatrolTimer++;
        }
    }

    public void MoveToPos(Vector2 position)
    {
        float dirX = LemonUtils.Sign(NPC.Center.DirectionTo(position).X, 1);
        if (MathF.Abs(NPC.velocity.X) < MovementSpeed)
        {
            NPC.velocity.X += dirX * Acceleration;
        }

        Point npcCenterTilePoint = NPC.Center.ToTileCoordinates();
        for (int x = 1; x < 3; x++)
        {
            Point pointInFront = new Point(2 * LemonUtils.Sign(NPC.velocity.X, 1), 0);
            Point tilePointInFront = npcCenterTilePoint + pointInFront;
            Tile tileInFront = Framing.GetTileSafely(tilePointInFront);
            if (tileInFront.HasTile && IsGrounded())
            {
                NPC.velocity.Y -= JumpHeight;
                break;
            }

        }
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
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
        StateControl();
        //Main.NewText(NPC.target);
        Dust.NewDustPerfect(CurrentPatrolPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;

        /*Dust.NewDustPerfect(LastSeenTargetPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
        for (int i = 0; i < 32; i++)
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
