using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Death.DeathKnightCaptain;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using static Microsoft.Xna.Framework.MathHelper;

namespace NeoParacosm.Content.NPCs.Bosses.DeathKnightCaptain;

// This boss is spread across multiple files
// This file contains primarily AI and Attack logic

[AutoloadBossHead]
public partial class DeathKnightCaptain : ModNPC
{
    #region Attack Fields and Data
    ref float AITimer => ref NPC.ai[0];

    /// <summary>
    /// The current attack being executed.
    /// </summary>
    public float Attack
    {
        get { return NPC.ai[1]; }
        private set
        {
            int diffMod = -1; // One less attack if not in expert
            if (Main.expertMode)
            {
                diffMod = 0;
            }
            int maxVal = Enum.GetValues(typeof(Attacks)).Length - 1;
            if (Phase == 1)
            {
                maxVal = Enum.GetValues(typeof(Attacks2)).Length - 1;
            }
            if (Phase == 2)
            {
                maxVal = Enum.GetValues(typeof(Attacks3)).Length - 1;
            }

            if (value > maxVal + diffMod || value < 0)
            {
                NPC.ai[1] = 0;
            }
            else
            {
                NPC.ai[1] = value;
            }
        }
    }

    // Counts down from attackDurations[Attack].
    ref float AttackTimer => ref NPC.ai[2];

    /// <summary>
    /// Used for counting whatever during attacks.
    /// Also used for misc purposes in certain attacks like remembering values or acting as a flag.
    /// </summary>
    ref float AttackCount => ref NPC.ai[3];
    float AttackCount2 = 0;

    /// <summary>
    /// Attack duration of the current attack being executed.
    /// Counts down and switches attacks when equal to 0
    /// </summary>
    float attackDuration = 0;

    /// <summary>
    /// Attack durations indexed by Attack field
    /// </summary>
    readonly int[] attackDurations = [540, 900, 750, 900, 360];
    readonly int[] attackDurations2 = [600, 1200, 1200, 1200, 1200, 1380, 1200, 1080, 1800];

    /// <summary>
    /// Attacks that can be performed (order matters)
    /// </summary>
    public enum Attacks
    {
        SpearThrowing,
        BombsAndBallLightning,
        Dashing,
        LightningSpearsDirect,
        Tired
    }

    public enum Attacks2
    {

    }

    public enum Attacks3
    {

    }

    public int Phase { get; private set; } = 0;
    bool reachedSecondPhase = false;

    bool doPhaseTransition = false;
    int phaseTransitionTimer = 0;

    bool reachedFinalPhase = false;
    #endregion

    Vector2 targetPosition = Vector2.Zero;
    Vector2 targetPosition2 = Vector2.Zero;

#pragma warning disable IDE1006 // Naming Styles
    public Player player { get; private set; }
#pragma warning restore IDE1006 // Naming Styles

    public override void AI()
    {
        doDrawPredictiveLaser = false;
        //attackDurations[0] = 540;
        //Main.NewText(AttackTimer);
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];

        if (AITimer < INTRO_DURATION)
        {
            Intro();
            AITimer++;
            return;
        }

        PassiveDust();
        DespawnCheck();

        if (doPhaseTransition)
        {
            PhaseTransition();
            return;
        }

        AttackControl();
        AITimer++;
    }

    void AttackControl()
    {
        if (Phase == 0)
        {
            switch (Attack)
            {
                case (int)Attacks.SpearThrowing:
                    Attack_SpearThrowing();
                    break;
                case (int)Attacks.BombsAndBallLightning:
                    Attack_BombsAndBallLightning();
                    break;
                case (int)Attacks.Dashing:
                    Attack_Dashing();
                    break;
                case (int)Attacks.LightningSpearsDirect:
                    Attack_LightningSpearsDirect();
                    break;
                case (int)Attacks.Tired:
                    Attack_Tired();
                    break;
            }
        }
        else if (Phase == 1)
        {
            /*switch (Attack)
            {
                case (int)Attacks2.Pillars:
                    Attack_Pillars();
                    break;

            }*/
        }

        attackDuration--;
        if (attackDuration <= 0)
        {
            SwitchAttacks();
        }
    }

    void SwitchAttacks()
    {
        Attack++;
        if (Phase == 0)
        {
            //Attack = 3;
        }

        if (Phase == 0)
        {
            attackDuration = attackDurations[(int)Attack];
        }
        else if (Phase == 1)
        {
            attackDuration = attackDurations2[(int)Attack];
        }

        AttackCount = 0;
        AttackCount2 = 0;
        AttackTimer = 0;
        NPC.ShowNameOnHover = true;
        NPC.dontTakeDamage = false;
        NPC.Opacity = 1f;
    }

    void DespawnCheck()
    {
        if (player.dead || !player.active || NPC.Center.Distance(player.MountedCenter) > 5000)
        {
            NPC.active = false;
            NPC.life = 0;
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }
    }

    const int INTRO_DURATION = 300;
    void Intro()
    {
        switch (AITimer)
        {
            case < 180:
                canFallThroughPlatforms = true;
                NPC.Center = player.Center - Vector2.UnitY * 200;
                NPC.Opacity = 0f;
                if (AITimer % 10 == 0)
                {
                    SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { PitchRange = (-0.2f, 0.5f), Volume = 0.6f }, NPC.Center);
                }
                Dust.NewDustPerfect(
                    NPC.RandomPos(32, 32),
                    DustType<StreakDust>(),
                    Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(2, 4),
                    newColor: Color.LightYellow
                    ).noGravity = true;
                NPC.rotation = -PiOver2;
                SetFrame(Crouching1);
                break;
            case 180:
                LemonUtils.DustBurst(20, NPC.Center, DustType<StreakDust>(), 10, 10, 0.5f, 2f, Color.LightYellow);
                break;
            case < 210:
                NPC.Opacity = 1f;
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(
                        NPC.RandomPos(),
                        DustType<StreakDust>(),
                        Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(8, 12),
                        newColor: Color.LightYellow
                        ).noGravity = true;
                }
                break;
            case 210:
                NPC.velocity = Vector2.UnitY * 60;
                if (LemonUtils.NotClient())
                {
                    Spawn_Lightning(NPC.Center - Vector2.UnitY * 600, 1800);
                }
                SetFrame(ArmFrontDashing);
                break;
            case < 240:
                SpawnDust();
                break;
            case < 300:
                canFallThroughPlatforms = false;
                NPC.rotation = Utils.AngleLerp(NPC.rotation, 0f, 1 / 10f);
                SetFrame(StandingNormal);
                break;
        }
        //Attack = 0;
        float clampedProgress = Clamp(AITimer / 180f, 0f, 1f);
        if (AITimer % 4 == 0 && AITimer < 180)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = Vector2.UnitY.RotatedBy(i * PiOver4 + Main.rand.NextFloat(-Pi / 6f, Pi / 6f));
                LemonUtils.QuickProj(
                    NPC,
                    NPC.Center,
                    dir,
                    ProjectileType<HolyLightningSmall>(),
                    ai0: 0,
                    ai1: Main.rand.NextFloat(120 * 0.75f, 120 * 1.25f) * clampedProgress
                    );
            }
        }
        attackDuration = attackDurations[(int)Attack];
    }

    void PhaseTransition()
    {
        switch (phaseTransitionTimer)
        {
            case 0:
                NPC.Opacity = 1f;
                NPC.ShowNameOnHover = true;
                TeleportEffect(8, 6, 6);
                SetFrame(Tired1);
                LookTowards(NPC.Center + Vector2.UnitX * LemonUtils.Sign(player.Center.X - NPC.Center.X, 1));
                break;
            case < 60:

                break;
            case 60:
                SetFrame(Tired2);
                break;
            case < 240:
                float lifeLerpT = (phaseTransitionTimer - 60f) / (240f - 60f - 1f);
                NPC.life = (int)Lerp(1, NPC.lifeMax, lifeLerpT);
                SetFrame(Crouching1);
                int dustCount = phaseTransitionTimer / 60;
                for (int i = 0; i < dustCount; i++)
                {
                    Dust.NewDustPerfect(
                        NPC.RandomPos(16 * dustCount, 16 * dustCount),
                        DustType<FireDust>(),
                        -Vector2.UnitY * Main.rand.NextFloat(0, 2f * dustCount),
                        newColor: Color.Black,
                        Scale: Main.rand.NextFloat(0.25f, 0.6f)
                        );
                }
                break;
            case < 270:
                SetFrame(StandingNormal);
                break;
            case < 280:
                SetFrame(Crouching1);
                break;
            case < 370:
                int dustCount2 = phaseTransitionTimer / 60;
                for (int i = 0; i < dustCount2; i++)
                {
                    Vector2 pos = NPC.Center + new Vector2(Main.rand.NextFloat(-32 * dustCount2, 32 * dustCount2), Main.rand.NextFloat(850, 950));
                    Dust.NewDustPerfect(
                        pos,
                        DustType<FireDust>(),
                        -Vector2.UnitY * Main.rand.NextFloat(30, 60f * dustCount2),
                        newColor: Color.Black,
                        Scale: Main.rand.NextFloat(2f, 2f + 0.3f * dustCount2)
                        );
                }
                break;
            case 370:
                doPhaseTransition = false;
                NPC.dontTakeDamage = false;
                break;
        }

        phaseTransitionTimer++;
        Attack = 0;
    }

    void Attack_SpearThrowing()
    {
        LookTowards(player.Center);
        switch (AttackTimer)
        {
            case 90:
                if (LemonUtils.NotClient())
                {
                    float distance = 700 - ((LemonUtils.GetDifficulty() - 1) * 100);
                    float randomRot = PiOver2 * Main.rand.Next(-1, 2);
                    if (LemonUtils.IsHard())
                    {
                        randomRot = PiOver4 * Main.rand.Next(-2, 3);
                    }
                    targetPosition = player.Center + NPC.DirectionTo(player.Center).RotatedBy(randomRot) * distance;
                    //targetPosition = player.Center - Vector2.UnitY.RotatedBy(randomRot) * distance;
                }
                NPC.netUpdate = true;
                targetPosition2 = NPC.Center;
                AttackCount = 0; // Used for lerping position
                GoInvisible();
                break;
            case > 60:
                SpawnDust();
                NPC.Center = Vector2.Lerp(targetPosition2, targetPosition, AttackCount / 30f);
                AttackCount++;
                break;
            case 60:
                if (LemonUtils.NotClient())
                {
                    Spawn_SmallLightningBurst(128, 224);
                }
                GoVisible();
                SetFrame(ArmUpNormal2);
                break;
            case > 30:
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(
                        NPC.RandomPos(),
                        DustType<StreakDust>(),
                        Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(8, 12),
                        newColor: Color.LightYellow
                        ).noGravity = true;
                }
                break;
            case 30:
                SetFrame(ArmFrontNormal2);

                if (LemonUtils.NotClient())
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Spawn_HolySpear(NPC.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * 15, 15, 0.2f, 15);
                    }
                    Spawn_HolySpear(NPC.DirectionTo(player.Center) * 15, 15, 0.2f, 15);
                }
                SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (-0.2f, 0.2f) }, NPC.Center);
                SetFrame(ArmFrontNormal2);
                //TeleportEffect(8, 6, 6);
                break;
            case 15:
                SetFrame(StandingNormal);
                break;
            case > 0:
                break;
            case 0:
                AttackTimer = 90;
                return;
        }

        AttackTimer--;
    }

    void Attack_BombsAndBallLightning()
    {
        LookTowards(player.Center);
        switch (AttackTimer)
        {
            case 180: // Crouch
                SetFrame(Crouching1);
                break;
            case > 120: // Spawn charging dust, slow down if moving
                NPC.velocity *= 0.94f;
                for (int i = 0; i < 3 * AttackCount / 20f; i++)
                {
                    Dust.NewDustPerfect(
                        NPC.RandomPos(),
                        DustType<StreakDust>(),
                        Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(2, 3) * (AttackCount / 8f),
                        newColor: Color.LightYellow
                        ).noGravity = true;
                }
                AttackCount++;
                break;
            case 120: // Go invisible, spawn lightning ball 
                GoInvisible();
                NPC.velocity = Vector2.Zero;
                AttackCount = 0;
                if (LemonUtils.NotClient())
                {
                    if (AttackCount2 == 1)
                    {
                        Spawn_LightningBall(NPC.Center, NPC.DirectionTo(player.Center) * 2, 60, 720, 160);
                    }
                    float randDir = Main.rand.NextSign();
                    targetPosition = player.Center + new Vector2(randDir * 1000, -400); // start position
                    targetPosition2 = player.Center + new Vector2(-randDir * 1000, -400); // end position
                }
                NPC.netUpdate = true;
                break;
            case > 60: // Move from one target pos to the next, spawning bombs
                LookTowards(targetPosition2);
                NPC.Center = Vector2.Lerp(targetPosition, targetPosition2, AttackCount / 60f);
                if (AttackTimer % 10 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        Spawn_Bomb(-Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-Pi / 6f, Pi / 6f) * 3), 60);
                    }
                }
                SpawnDust();
                AttackCount++;
                break;
            case 60: // Go visible
                GoVisible();
                AttackCount = 0;
                break;
            case > 30: // Prepare spears in hard
                if (LemonUtils.IsHard())
                {
                    SetFrame(ArmUpNormal2);
                }
                break;
            case 30: // Throw spears in hard
                if (LemonUtils.IsHard())
                {
                    SetFrame(ArmFrontNormal2);
                    if (LemonUtils.NotClient())
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Spawn_HolySpear(NPC.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * 15, 15, 0.2f, 15);
                        }
                        Spawn_HolySpear(NPC.DirectionTo(player.Center) * 15, 15, 0.2f, 15);
                    }
                }
                break;
            case 15:
                SetFrame(StandingNormal);
                break;
            case > 0:
                break;
            case 0:
                AttackTimer = 180;
                AttackCount2++;
                return;
        }

        AttackTimer--;
    }

    void Attack_Dashing()
    {
        //Main.NewText(AttackTimer);
        switch (AttackTimer)
        {
            case 150:
                if (LemonUtils.NotClient())
                {
                    AttackCount = Main.rand.NextSign(); // Side
                    //AttackCount.NewText();
                }
                NPC.netUpdate = true;
                NPC.velocity = Vector2.Zero;
                LookTowards(player.Center);
                TeleportEffect(12, 8, 8);
                targetPosition = player.Center + new Vector2(AttackCount * 500, 0);
                NPC.Center = targetPosition;
                TeleportEffect(12, 8, 8);
                LemonUtils.DustLine(NPC.Center, player.Center + new Vector2(AttackCount * 500, 0), DustType<FireDust>(), 5, 0.75f, Color.Black);
                SetFrame(Crouching1);
                //SoundEngine.PlaySound(SoundID.NPCDeath10 with { PitchRange = (-1f, -0.8f), MaxInstances = 2}, NPC.Center);
                break;
            case > 105:
                doDrawPredictiveLaser = true;
                if (LemonUtils.NotClient() && AttackTimer % 4 == 0)
                {
                    Spawn_SmallLightning(NPC.Center, Vector2.UnitY.RotatedByRandom(6.28f), 64f, 90f);
                }
                targetPosition = player.Center + new Vector2(AttackCount * 500, 0);
                NPC.Center = targetPosition;
                targetPosition2 = player.Center + new Vector2(500 * -AttackCount, player.velocity.Y * 60);
                LookTowards(targetPosition2);
                break;
            case > 75:
                LookTowards(targetPosition2);
                break;
            case 75:
                //GoInvisible();
                SetFrame(Dashing);
                if (LemonUtils.NotClient())
                {
                    Vector2 dir = NPC.Center.DirectionTo(targetPosition2);
                    Spawn_SmallLightning(NPC.Center - dir * 500, dir, 2000, 2002);
                }
                break;
            case > 60:
                bool extraCondition = LemonUtils.IsHard() || attackDuration <= 300;
                if (AttackTimer % 2 == 0 && extraCondition)
                {
                    if (LemonUtils.NotClient())
                    {
                        Spawn_Lightning(NPC.Center + new Vector2(Main.rand.NextFloat(-48, 48), -600), 1800);
                    }
                }
                NPC.Center = Vector2.Lerp(targetPosition, targetPosition2, AttackCount2 / 15f);
                SpawnDust();
                AttackCount2++;
                break;
            case 60:
                SetFrame(Crouching1);
                NPC.Center = targetPosition2;
                //GoVisible();
                bool extraCondition2 = LemonUtils.IsHard() || attackDuration <= 600;
                if (LemonUtils.NotClient() && extraCondition2)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Spawn_LightningWarning(NPC.Center + new Vector2(-AttackCount * 100 * (i + 1), -1200), 10 * i, 2400);
                    }
                }
                break;
            case > 0:
                LookTowards(NPC.Center + Vector2.UnitX);
                break;
            case 0:
                AttackTimer = 150;
                AttackCount2 = 0;
                return;
        }

        AttackTimer--;
    }

    void Attack_LightningSpearsDirect()
    {
        switch (AttackTimer)
        {
            case 120:
                TeleportEffect(8, 4, 4);
                if (LemonUtils.NotClient())
                {
                    int attCount = 0;
                    do
                    {
                        targetPosition = player.Center + Main.rand.NextVector2CircularEdge(400, 400);
                        attCount++;
                    }
                    while (!Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2) && attCount < 100);
                }

                NPC.netUpdate = true;
                NPC.Center = targetPosition;
                TeleportEffect(8, 4, 4);
                SetFrame(ArmUpNormal2);
                break;
            case > 0:
                LookTowards(player.Center);
                if (AttackTimer % 90 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        for (int i = 0; i < AttackCount; i++)
                        {
                            Spawn_HolyLightningSpear(
                                NPC.Top - Vector2.UnitY.RotatedBy(PiOver4 * i) * 120,
                                50 + LemonUtils.GetDifficulty() * 10,
                                180,
                                30 + 10 * i);
                        }
                    }

                }

                if (AttackTimer == 60)
                {
                    SetFrame(ArmFrontNormal2);
                }
                break;
            case 0:
                AttackTimer = 120;
                AttackCount++;
                return;
        }

        AttackTimer--;
    }

    void Attack_Tired()
    {
        switch (AttackTimer)
        {
            case 120:
                LookTowards(NPC.Center + Vector2.UnitX * 5 * LemonUtils.Sign(NPC.DirectionTo(player.Center).X, 1));
                SetFrame(Tired1);
                break;
            case 60:
                SetFrame(Tired2);
                break;
            case 0:
                AttackTimer = 120;
                return;
        }

        AttackTimer--;
    }

    void Spawn_LightningBall(Vector2 pos, Vector2 velocity, int waitTime, int duration, float avgLength)
    {
        LemonUtils.QuickProj(
            NPC,
            pos,
            velocity,
            ProjectileType<LightningBall>(),
            ai0: waitTime,
            ai1: duration,
            ai2: avgLength
            );
    }

    void Spawn_HolySpear(Vector2 velocity, float fireSpeed = 15f, float fallSpeed = 0.2f, float timeBeforeFall = 15f)
    {
        LemonUtils.QuickProj(
            NPC,
            NPC.Top,
            velocity,
            ProjectileType<HolySpear>(),
            ai0: fireSpeed,
            ai1: fallSpeed,
            ai2: timeBeforeFall
            );
    }

    void Spawn_HolyLightningSpear(Vector2 pos, float speed, int timeLeft, int waitTime)
    {
        LemonUtils.QuickProj(
            NPC,
            pos,
            Vector2.UnitX * speed,
            ProjectileType<HolyLightningSpear>(),
            ai0: timeLeft,
            ai1: waitTime
            );
    }

    void Spawn_Bomb(Vector2 velocity, int WaitTime)
    {
        LemonUtils.QuickProj(
            NPC,
            NPC.Center,
            velocity,
            ProjectileType<DarkIncendiaryProjHostile>(),
            ai0: WaitTime,
            ai1: NPC.target,
            ai2: 15f
            );
    }

    void Spawn_LightningWarning(Vector2 pos, int warningDuration, float length)
    {
        LemonUtils.QuickProj(
            NPC,
            pos,
            Vector2.Zero,
            ProjectileType<HolyLightningWarningProj>(),
            ai1: warningDuration,
            ai2: length
            );
    }

    void Spawn_SmallLightningBurst(float minLength, float maxLength)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 dir = Vector2.UnitY.RotatedBy(i * PiOver4 + Main.rand.NextFloat(-Pi / 6f, Pi / 6f));
            LemonUtils.QuickProj(
                NPC,
                NPC.Center,
                dir,
                ProjectileType<HolyLightningSmall>(),
                ai0: 0,
                ai1: Main.rand.NextFloat(minLength, maxLength)
                );
        }
    }

    void Spawn_SmallLightning(Vector2 pos, Vector2 dir, float minLength, float maxLength)
    {
        LemonUtils.QuickProj(
            NPC,
            pos,
            dir,
            ProjectileType<HolyLightningSmall>(),
            ai0: 0,
            ai1: Main.rand.NextFloat(minLength, maxLength)
            );

    }

    void Spawn_Lightning(Vector2 pos, float length)
    {
        LemonUtils.QuickProj(
            NPC,
            pos,
            Vector2.Zero,
            ProjectileType<HolyLightning>(),
            ai1: length
            );
    }
}
