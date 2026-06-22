using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Core.Systems.Data;
using Terraria.Audio;
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
    readonly int[] attackDurations = [600, 1080, 1080, 1080, 1320];
    readonly int[] attackDurations2 = [1600, 1200, 1200, 1200, 1200, 1380, 1200, 1080, 1800];
    readonly int[] attackDurations3 = [960, 600, 600, 1200, 1200, 1800];

    /// <summary>
    /// Attacks that can be performed (order matters)
    /// </summary>
    public enum Attacks
    {
        BallsNLightning,
        MeatballsWithFire,
        CursedFlamethrower,
        FlameWallsAndSpinning,
        DashingWithBalls,
    }

    public enum Attacks2
    {
        Pillars,
        CirclingIchor,
        Slamming,
        Dashing,
        LightningAndPillars,
        Spirits,
        LightningAndBalls,
        FlameLasers,
        TeleportDashes,
    }

    public enum Attacks3
    {
        FinalIntro,
        DashExplosions,
        IchorSpam,
        Lasers,
        LaserSpirits,
        Final_Mayhem
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

#pragma warning disable IDE1006 // Naming Styles
    public Player player { get; private set; }
#pragma warning restore IDE1006 // Naming Styles
    Vector2 NPCToPlayer => NPC.DirectionTo(player.Center);

    Vector2 ArenaCenter => DarkCataclysmSystem.DCEffectNoFogPosition == Vector2.Zero ? player.Center : DarkCataclysmSystem.DCEffectNoFogPosition;

    public override void AI()
    {
        //Main.NewText(AttackTimer);
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];
        NPC.width = (int)(284 * NPC.scale);
        NPC.height = (int)(416 * NPC.scale);
        if (AITimer < 540)
        {
            Intro();
            AITimer++;
            return;
        }

        DespawnCheck();
        AttackControl();
        AITimer++;
    }


    void AttackControl()
    {
        if (Phase == 0)
        {
            /*switch (Attack)
            {
                case (int)Attacks.BallsNLightning:
                    Attack_BallsNLightning();
                    break;

            }*/
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

    void Intro()
    {

        attackDuration = attackDurations[(int)Attack];
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
}
