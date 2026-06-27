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
    readonly int[] attackDurations = [720, 900, 1080, 1080, 1320];
    readonly int[] attackDurations2 = [1600, 1200, 1200, 1200, 1200, 1380, 1200, 1080, 1800];
    readonly int[] attackDurations3 = [960, 600, 600, 1200, 1200, 1800];

    /// <summary>
    /// Attacks that can be performed (order matters)
    /// </summary>
    public enum Attacks
    {
        SpearThrowing,
        BombsAndBallLightning,
    }

    public enum Attacks2
    {

    }

    public enum Attacks3
    {

    }

    public int Phase { get; private set; } = 0;
    bool reachedSecondPhase = false;

    bool reachedFinalPhase = false;

    bool doDepleteHealth = false;
    int phase3Timer = 0;

    #endregion
    #region Constants

    // Animation frame constants
    const int MOUTH_CLOSED = 0;
    const int MOUTH_OPEN = 1;

    const int LEG_STANDARD = 0;
    const int LEG_ATTACK = 1;
    #endregion

    Vector2 targetPosition = Vector2.Zero;
    Vector2 targetPosition2 = Vector2.Zero;

#pragma warning disable IDE1006 // Naming Styles
    public Player player { get; private set; }
#pragma warning restore IDE1006 // Naming Styles

    public override void AI()
    {
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
        else if (Phase == 2)
        {
            /*switch (Attack)
            {
                case (int)Attacks3.FinalIntro:
                    Final_Intro();
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
        if (Phase == 2)
        {
            //Attack = 5;
        }

        if (Phase == 0)
        {
            attackDuration = attackDurations[(int)Attack];
        }
        else if (Phase == 1)
        {
            attackDuration = attackDurations2[(int)Attack];
        }
        else if (Phase == 2)
        {
            attackDuration = attackDurations3[(int)Attack];
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
                LemonUtils.QuickProj(
                    NPC,
                    NPC.Center - Vector2.UnitY * 600,
                    Vector2.Zero,
                    ProjectileType<HolyLightning>(),
                    ai1: 1800
                    );
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
                        Spawn_LightningBall(NPC.Center, NPC.DirectionTo(player.Center) * 2, 60, 800, 160);
                    }
                    float randDir = Main.rand.NextDirectionInt();
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
                        Spawn_Bomb(-Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-Pi / 6f, Pi / 6f) * 3), 30);
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
}
