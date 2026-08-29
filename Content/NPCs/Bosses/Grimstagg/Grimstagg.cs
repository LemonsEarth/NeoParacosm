using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Death.DeathKnightCaptain;
using NeoParacosm.Core.Systems.Assets;
using System.Linq;
using Terraria.Audio;
using static Microsoft.Xna.Framework.MathHelper;

namespace NeoParacosm.Content.NPCs.Bosses.Grimstagg;

// This boss is spread across multiple files
// This file contains primarily AI and Attack logic

[AutoloadBossHead]
public partial class Grimstagg : ModNPC
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
    readonly int[] attackDurations = [540, 900, 600, 750, 900, 360];
    readonly int[] attackDurations2 = [720, 720, 1800, 2400, 960, 900, 360];

    /// <summary>
    /// Attacks that can be performed (order matters)
    /// </summary>
    public enum Attacks
    {

    }

    public enum Attacks2
    {

    }

    public int Phase { get; private set; } = 0;

    bool doPhaseTransition = false;
    int phaseTransitionTimer = 0;
    #endregion

    Vector2 targetPosition = Vector2.Zero;
    Vector2 targetPosition2 = Vector2.Zero;

#pragma warning disable IDE1006 // Naming Styles
    public Player player { get; private set; }
#pragma warning restore IDE1006 // Naming Styles

    public override void AI()
    {
        NPC.rotation = 0;
        HeadRotation = NPC.rotation;
        //attackDurations2[3] = 2400;
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
            /*switch (Attack)
            {
                case (int)Attacks.SpearThrowing:
                    Attack_SpearThrowing();
                    break;
            }*/
        }
        else if (Phase == 1)
        {
            /*switch (Attack)
            {
                case (int)Attacks2.DashingSuper:
                    Attack_DashingSuper();
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
            //if (Attack > 3)
            //    Attack = 2;
        }
        if (Phase == 1)
        {
            //Attack = 0;
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

        attackDuration = attackDurations[(int)Attack];
    }

    void PhaseTransition()
    {

        phaseTransitionTimer++;
        Attack = 0;
    }
}
