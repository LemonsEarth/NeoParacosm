using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using static Microsoft.Xna.Framework.MathHelper;

namespace NeoParacosm.Content.NPCs.Bosses.Dreadlord;

[AutoloadBossHead]
public class Dreadlord : ModNPC
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
    /// whether NPC is to the left (-1) or right (1) of the player
    /// </summary>
    int playerSide = 1;
    float PlayerSide
    {
        get => playerSide;
        set
        {
            if (value >= 0)
            {
                playerSide = 1;
            }
            else
            {
                playerSide = -1;
            }
        }
    }

    /// <summary>
    /// Attack duration of the current attack being executed.
    /// Counts down and switches attacks when equal to 0
    /// </summary>
    float attackDuration = 0;

    /// <summary>
    /// Attack durations indexed by Attack field
    /// </summary>
    int[] attackDurations = { 600, 1080, 1080, 1080, 1320 };

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

    // Wing animation stuff
    int wingFrame = 0;
    int wingAnimTimer = 0;

    // Whether eye lasers should be active
    bool GreenLaserEnabled = false;
    bool YellowLaserEnabled = false;
    #endregion

    #region Constants
    const int INTRO_DURATION = 60;

    // Animation frame constants
    const int HEAD_MOUTH_CLOSED = 0;
    const int HEAD_MOUTH_OPEN = 1;

    const int LEG_STANDARD = 0;
    const int LEG_ATTACK = 1;
    #endregion

    #region Body Parts
    public DreadlordBodyPart HeadCorrupt { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart HeadCrimson { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart WingCorrupt { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart WingCrimson { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart LegCorrupt { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart LegCrimson { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart Body { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart BackLegs { get; private set; } = new DreadlordBodyPart();

    public static Asset<Texture2D> neckTextureCorrupt { get; private set; }
    public static Asset<Texture2D> neckTextureCrimson { get; private set; }

    bool shaderIsActive = false;

    #endregion

    int facingDirection = 1;

    Vector2 targetPosition = Vector2.Zero;
    public Player player { get; private set; }
    Vector2 NPCToPlayer => NPC.DirectionTo(player.Center);
    float ProjDamage => NPC.damage / 4;

    Vector2 arenaCenter => WorldDataSystem.DCEffectNoFogPosition == Vector2.Zero ? player.Center : WorldDataSystem.DCEffectNoFogPosition;

    /// <summary>
    /// The leg being controlled in attacks that utilize a single leg at a time
    /// </summary>
    DreadlordBodyPart targetLeg;

    public override void Load()
    {
        neckTextureCorrupt = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordNeckCorrupt");
        neckTextureCrimson = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordNeckCrimson");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { };
        /*{
            PortraitScale = 0.2f,
            PortraitPositionYOverride = -150
        };*/
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Bestiary")),
            });
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {

    }

    public override void SetDefaults()
    {
        NPC.width = 284;
        NPC.height = 416;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.Opacity = 1f;
        NPC.lifeMax = 200000;
        NPC.defense = 60;
        NPC.damage = 100;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 300000;
        NPC.noTileCollide = false;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 10;
        NPC.SpawnWithHigherTime(30);

        HeadCorrupt.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordHeadCorrupt");
        HeadCrimson.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordHeadCrimson");
        LegCorrupt.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordLegCorrupt");
        LegCrimson.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordLegCrimson");
        WingCorrupt.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordWingCorrupt");
        WingCrimson.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordWingCrimson");
        Body.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordBody");
        BackLegs.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordBackLegs");

        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Common/Assets/Audio/Music/EmbodimentOfEvilReborn");
        }

    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.NP().SetDamageReductions(
            (DamageClass.Melee, 10f),
            (DamageClass.Ranged, 15f),
            (DamageClass.Magic, 15f),
            (DamageClass.Summon, 10f),
            (DamageClass.SummonMeleeSpeed, 60f)
            );
    }

    public override bool CheckActive()
    {
        return false;
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.5f);
    }

    public override void AI()
    {
        WorldDataSystem.DreadlordAlive = true;
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];

        SetDefaultBodyPartPositions();

        if (AITimer < 540)
        {
            Intro();
            AITimer++;
            return;
        }
        DespawnCheck();
        SetBodyPartPositions();
        AttackControl();
        ArenaControl();
        AnimateWings(8);
        AITimer++;
    }


    void Intro()
    {
        //NPC.dontTakeDamage = true;

        if (WorldDataSystem.DCEffectNoFogPosition == Vector2.Zero)
        {
            WorldDataSystem.DCEffectNoFogPosition = NPC.Center;
        }

        switch (AITimer)
        {
            case < 120:
                SetBodyPartPositions(headLerpSpeed: 1 / 5f, legLerpSpeed: 1 / 5f, bodyLerpSpeed: 1 / 10f);
                if (NPC.Center.Y < WorldDataSystem.DCEffectNoFogPosition.Y)
                {
                    NPC.velocity.Y += 0.5f;
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { MaxInstances = 1, PitchVariance = 1.0f, Volume = 0.5f, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest }, NPC.Center);

                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDustDirect(NPC.RandomPos(300, 200), 2, 2, DustID.CursedTorch, Scale: 2.5f).noGravity = true;
                        Dust.NewDustDirect(NPC.RandomPos(300, 200), 2, 2, DustID.Ichor, Scale: 2.5f).noGravity = true;
                    }
                }
                else
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.noTileCollide = true;
                }
                break;
            case 120:
                LemonUtils.QuickCameraFocus(NPC.Center, () => AITimer > 480 || !NPC.active);
                break;
            case < 180:
                SetBodyPartPositions(HeadCorrupt.DefaultPosition + new Vector2(0, 40),
                                     HeadCrimson.DefaultPosition + new Vector2(0, 40),
                                     LegCorrupt.DefaultPosition,
                                     LegCrimson.DefaultPosition,
                                     Body.DefaultPosition + new Vector2(0, 20),
                                     1 / 10f,
                                     1 / 10f,
                                     1 / 10f
                                     );
                break;
            case 210:
                NPC.noTileCollide = true;
                PlayRoar();
                LemonUtils.QuickScreenShake(NPC.Center, 60f, 8f, 360, 2000f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    LemonUtils.QuickPulse(NPC, Vector2.Lerp(HeadCorrupt.Position, HeadCrimson.Position, 0.5f), 2, 15, 5);
                }
                break;
            case 360:
                PlayRoar(0.3f);
                break;
            case < 480 and > 210:
                float speed = AITimer < 360 ? 2 : 4;
                float size = AITimer < 360 ? 15 : 25;
                if (AITimer < 360)
                {
                    if (AITimer % 15 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            LemonUtils.QuickPulse(NPC, NPC.Center, 2, 15, 5);
                        }
                    }

                    if (AITimer % 20 == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (LemonUtils.NotClient())
                            {
                                Vector2 randPos = NPC.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), -1200);
                                LemonUtils.QuickProj(
                                    NPC,
                                    randPos,
                                    Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-Pi / 6, Pi / 6)) * Main.rand.NextFloat(7, 12),
                                    ProjectileType<CursedFlameSphere>(),
                                    ProjDamage
                                    );
                            }
                        }
                    }
                    if (AITimer % 10 == 0)
                    {
                        if (LemonUtils.NotClient())
                        {
                            LightningAroundPlayer(900, -1500, 120, 3000);
                        }
                    }
                }
                else
                {
                    if (AITimer % 15 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            LemonUtils.QuickPulse(NPC, NPC.Center, 3, 25, 5);
                        }
                    }

                    if (AITimer % 15 == 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (LemonUtils.NotClient())
                            {
                                Vector2 randPos = NPC.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), -1200);
                                LemonUtils.QuickProj(
                                    NPC,
                                    randPos,
                                    Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-Pi / 6, Pi / 6)) * Main.rand.NextFloat(7, 12),
                                    ProjectileType<CursedFlameSphere>(),
                                    ProjDamage
                                    );
                            }
                        }
                    }
                }

                SetBodyPartPositions(HeadCorrupt.DefaultPosition - new Vector2(0, 50),
                                    HeadCrimson.DefaultPosition - new Vector2(0, 50)
                                    );
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                if (AttackTimer % 4 == 0)
                {
                    SetBodyPartPositions(HeadCorrupt.DefaultPosition + Main.rand.NextVector2Circular(24, 24),
                                         HeadCrimson.DefaultPosition + Main.rand.NextVector2Circular(24, 24),
                                         LegCorrupt.DefaultPosition + Main.rand.NextVector2Circular(24, 24),
                                         LegCrimson.DefaultPosition + Main.rand.NextVector2Circular(24, 24),
                                         Body.DefaultPosition + Main.rand.NextVector2Circular(24, 24)
                        );
                }
                //NPC.velocity = Main.rand.NextVector2Circular(2, 2);
                break;
            case < 540 and > 210:
                NPC.velocity = Vector2.Zero;
                SetBodyPartPositions();
                HeadCorrupt.CurrentFrame = HEAD_MOUTH_CLOSED;
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                break;
        }

        //NPC.velocity = Vector2.Zero;
        attackDuration = attackDurations[(int)Attack];
    }

    /// <summary>
    /// Spawns special Crimson Lost Souls around players that go too far from the boss or from the arena center
    /// </summary>
    void ArenaControl()
    {
        /*for (int i = 0; i < 64; i++)
        {
            Dust.NewDustDirect(NPC.Center + Vector2.UnitY.RotatedBy(i * (Pi * 2 / 64f)) * 1500, 2, 2, DustID.GemDiamond, Scale: 5f).noGravity = true;
        }*/

        if (LemonUtils.NotClient())
        {
            foreach (var plr in Main.ActivePlayers)
            {
                float distanceToNPC = plr.Distance(NPC.Center);
                float distanceToArena = plr.Distance(arenaCenter);

                float baseArenaDistance = WorldDataSystem.DCEffectNoFogDistance * 0.5f;
                float baseNPCDistance = 1500f * 0.5f;
                if (distanceToNPC > baseNPCDistance) // too far from boss
                {
                    float speedDen = Math.Clamp(distanceToNPC / baseNPCDistance, 1, 3);
                    if (AITimer % (60 / speedDen) == 0)
                    {
                        Vector2 pos = player.Center + NPC.Center.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * 400;
                        LemonUtils.QuickProj(
                            NPC,
                            pos,
                            pos.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * Main.rand.NextFloat(2, 4),
                            ProjectileType<ArenaBoundLostSoul>(),
                            ProjDamage * 2,
                            ai0: 300,
                            ai1: Main.rand.NextFloat(5, 8),
                            ai2: 160
                            );
                    }
                }

                if (distanceToArena > baseArenaDistance) // too far from arena
                {
                    float speedDen = Math.Clamp(distanceToArena / baseArenaDistance, 1, 3);
                    if (AITimer % (60 / speedDen) == 0)
                    {
                        Vector2 pos = player.Center + arenaCenter.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * 400;
                        LemonUtils.QuickProj(
                            NPC,
                            pos,
                            pos.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * Main.rand.NextFloat(4, 6),
                            ProjectileType<ArenaBoundLostSoul>(),
                            ProjDamage * 2,
                            ai0: 600,
                            ai1: Main.rand.NextFloat(8, 10),
                            ai2: 160
                            );
                    }
                }
            }
        }
    }

    void AttackControl()
    {
        switch (Attack)
        {
            case (int)Attacks.BallsNLightning:
                BallsNLightning();
                break;
            case (int)Attacks.MeatballsWithFire:
                MeatballsWithFire();
                break;
            case (int)Attacks.CursedFlamethrower:
                CursedFlamethrower();
                break;
            case (int)Attacks.FlameWallsAndSpinning:
                FlameWallsAndSpinning();
                break;
            case (int)Attacks.DashingWithBalls:
                DashingWithBalls();
                break;
        }

        attackDuration--;
        if (attackDuration <= 0)
        {
            SwitchAttacks();
        }
    }

    void BallsNLightning()
    {
        if (AttackTimer < 450)
        {
            SetBodyPartPositions(HeadCorrupt.DefaultPosition + Vector2.UnitY * 40,
                HeadCrimson.DefaultPosition - Vector2.UnitY * 32 * AttackCount + Main.rand.NextVector2Circular(24, 24));
        }
        else
        {
            SetBodyPartPositions(headLerpSpeed: 1 / 2f, legLerpSpeed: 1 / 2f, bodyLerpSpeed: 1 / 2f);
        }
        switch (AttackTimer)
        {
            case 600: // Choosing random position around arena center
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    targetPosition = arenaCenter + Main.rand.NextVector2Circular(600, 600);
                }
                NPC.netUpdate = true;
                break;
            case > 450: // Moving to target position
                NPC.MoveToPos(targetPosition, 0.04f, 0.04f, 0.1f, 0.1f);
                break;
            case > 420: // Slowing down, moving leg outward
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, ToRadians(60), 1 / 10f); // Move leg outward
                SetLegCorruptFrame(LEG_ATTACK);
                NPC.velocity *= 0.95f;
                break;
            case 420: // Spawn giant cursed sphere (explodes at 300), stop moving, roar
                NPC.velocity = Vector2.Zero;
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.05f, 420 - 300);
                }
                PlayRoar(0.3f);
                AttackCount++;
                break;
            case > 270: // Begin ichor lightning with lost souls from below
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                if (AttackTimer % 15 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(1200, -1500, 60, 3000);

                    if (LemonUtils.IsHard())
                    {
                        LemonUtils.QuickProj(
                            NPC,
                            player.Center + new Vector2(Main.rand.NextFloat(-800, 800), 800),
                            -Vector2.UnitY * 5,
                            ProjectileType<CrimsonLostSoul>(),
                            ProjDamage,
                            ai0: 60,
                            ai1: 180
                            );
                    }

                }

                if (AttackTimer % 30 == 0 && LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(
                        NPC,
                        HeadCrimson.Position,
                        Vector2.UnitY.RotatedByRandom(Pi * 2) * 6,
                        ProjectileType<IchorSphere>(),
                        ProjDamage,
                        ai0: 180,
                        ai1: 60,
                        ai2: 2
                        );
                }
                break;
            case 270: // Reset frames
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                SetLegCorruptFrame(LEG_STANDARD);
                AttackCount++;
                break;
            case > 240: // Move leg back
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, 0, 1 / 10f);
                break;
            case 240: // Spawning second giant cursed sphere (explodes at 120)
                SetLegCorruptFrame(LEG_ATTACK);
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.05f, 240 - 120);
                }
                break;
            case 210: // Roar and open crimson mouth
                PlayRoar(0.6f);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                AttackCount++;
                break;
            case > 120 and < 210: // Second wave of lightning
                LegCorrupt.Rotation = 0;
                if (AttackTimer % 10 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(1200, -1500, 90, 3000);
                }
                break;
            case 120: // Reset leg
                SetLegCorruptFrame(LEG_STANDARD);
                break;
            case > 60 and < 210: // Faster lightning
                if (AttackTimer % 5 == 0 && LemonUtils.NotClient())
                {
                    int lightningCount2 = LemonUtils.IsHard() ? 1 : 3;
                    if (LemonUtils.IsHard())
                    {
                        for (int i = 0; i < lightningCount2; i++)
                        {
                            LightningAroundPlayer(600, -1500, 90, 3000);
                        }
                    }
                }
                break;
            case > 0:

                break;
            case 0:
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                AttackTimer = 600;
                return;
        }

        AttackTimer--;
    }

    void MeatballsWithFire()
    {
        SetBodyPartPositions(headLerpSpeed: 0.8f, bodyLerpSpeed: 0.8f, legLerpSpeed: 0.8f);
        switch (AttackTimer)
        {
            case 1080: // Choose random position
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    targetPosition = arenaCenter + LemonUtils.RandomVector2Circular(600, 600, 100, 100);
                }
                NPC.netUpdate = true;
                PlayRoar(-0.2f);
                break;
            case > 780: // Move to random positions, firing cursed spheres at player
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                Vector2 headOffset = HeadCorrupt.MiscPosition1 + HeadCorrupt.MiscPosition1.DirectionTo(player.Center) * 100;
                HeadCorrupt.Position = Vector2.Lerp(HeadCorrupt.Position, headOffset, 1 / 10f);
                NPC.MoveToPos(targetPosition, 0.1f, 0.04f, 0.1f, 0.1f);
                if (AttackTimer > 840)
                {
                    if (LemonUtils.NotClient() && AttackTimer % 5 == 0)
                    {
                        LemonUtils.QuickProj(
                            NPC,
                            HeadCorrupt.MiscPosition1,
                            HeadCorrupt.MiscPosition1.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 6, Pi / 6)) * Main.rand.NextFloat(3, 6),
                            ProjectileType<CursedFlameSphere>(),
                            ProjDamage,
                            ai1: Main.rand.NextFloat(1.005f, 1.025f)
                            );
                    }
                }
                else
                {
                    SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                }
                break;
            case 780: // Predictive cursed sphere burst
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);

                for (int i = 0; i < 8; i++)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(
                            NPC,
                            HeadCorrupt.MiscPosition1,
                            HeadCorrupt.MiscPosition1.DirectionTo(player.Center + player.velocity * 75).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * Main.rand.NextFloat(3, 6),
                            ProjectileType<CursedFlameSphere>(),
                            ProjDamage,
                            ai1: Main.rand.NextFloat(1.005f, 1.025f)
                            );
                    }
                }
                break;
            case > 750: // Slowing down
                NPC.velocity *= 0.96f;
                break;
            case > 720: // Slowing down, move crimson leg outward
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                NPC.velocity *= 0.96f;
                SetLegCrimsonFrame(LEG_ATTACK);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, -Pi / 6, 1 / 10f);
                break;
            case 720: // Giant meatball (explodes at 480)
                if (LemonUtils.NotClient())
                {
                    GiantMeatball(LegCrimson.MiscPosition1, 720 - 480, 60, 360, 5);
                }
                break;
            case > 690: // Rotate leg back
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, 0, 1 / 10f);
                break;
            case 690: // Reset leg, calculate where NPC is relative to arena center, calculate new target pos (start firing angle)
                SetLegCrimsonFrame(LEG_STANDARD);
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                break;
            case > 480: // Firing cursed flames spheres
                Vector2 headOffset2 = HeadCorrupt.MiscPosition1 + HeadCorrupt.MiscPosition1.DirectionTo(targetPosition) * 100;
                HeadCorrupt.Position = Vector2.Lerp(HeadCorrupt.Position, headOffset2, 1 / 10f);
                if (AttackTimer % 5 == 0)
                {
                    for (int i = 0; i < 1 * LemonUtils.GetDifficulty(); i++)
                    {
                        if (LemonUtils.NotClient())
                        {
                            LemonUtils.QuickProj(
                                NPC,
                                HeadCorrupt.MiscPosition1,
                                Vector2.UnitY.RotatedByRandom(Pi * 2) * Main.rand.NextFloat(3, 6),
                                ProjectileType<CursedFlameSphere>(),
                                ProjDamage,
                                ai1: Main.rand.NextFloat(1.02f, 1.04f)
                                );
                        }
                    }
                }
                if (AttackTimer < 630)
                {
                    targetPosition = targetPosition.RotatedBy(ToRadians(3 * AttackCount), HeadCorrupt.MiscPosition1);
                }
                break;
            case 480: // Close mouth, scream on hard
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                PlayRoar(-0.2f);

                if (LemonUtils.IsHard())
                {
                    SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                    PlayRoar(0.3f);
                }
                break;
            case > 180: // Move above player, predictive burst shots, lightning on hard
                if (AttackTimer % 60 == 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (LemonUtils.NotClient())
                        {
                            LemonUtils.QuickProj(
                                NPC,
                                HeadCorrupt.MiscPosition1,
                                HeadCorrupt.MiscPosition1.DirectionTo(player.Center + player.velocity * 60).RotatedBy(Main.rand.NextFloat(-Pi / 6, Pi / 6)) * Main.rand.NextFloat(6, 10),
                                ProjectileType<CursedFlameSphere>(),
                                ProjDamage,
                                ai1: Main.rand.NextFloat(1.002f, 1.005f)
                                );
                        }
                    }
                }
                if (LemonUtils.IsHard())
                {
                    HeadCrimson.Position = HeadCrimson.DefaultPosition - Vector2.UnitY * 40 + Main.rand.NextVector2Circular(24, 24);
                    if (AITimer % 120 == 0)
                    {
                        PlayRoar(0.3f);
                    }
                    if (LemonUtils.NotClient() && AttackTimer % 20 == 0)
                    {
                        LightningAroundPlayer(600, -1500, 120, 3000);
                    }
                }

                NPC.MoveToPos(player.Center - Vector2.UnitY * 800, 0.1f, 0.2f, 0.4f, 0.4f);
                break;
            case 180: // Close mouth
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                break;
            case > 120: // Slow down
                EnableLasers(true);
                NPC.velocity *= 0.97f;
                break;
            case 120: // Dash
                EnableLasers(false);
                NPC.velocity = NPC.DirectionTo(player.Center) * 30;
                PlayRoar(-0.2f);
                PlayRoar(0.2f);
                break;
            case > 90:
                break;
            case > 30: // Move to player
                NPC.MoveToPos(player.Center, 0.1f, 0.05f, 0.3f, 0.3f);
                break;
            case > 0: // Slow down
                NPC.velocity *= 0.95f;
                break;
            case 0:
                AttackTimer = 1080;
                return;
        }

        AttackTimer--;
    }

    void CursedFlamethrower()
    {
        SetBodyPartPositions(headLerpSpeed: 0.8f, bodyLerpSpeed: 0.8f, legLerpSpeed: 0.8f);
        switch (AttackTimer)
        {
            case 1080: // Choose random position
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    targetPosition = arenaCenter + LemonUtils.RandomVector2Circular(600, 600, 200, 200);
                }
                NPC.netUpdate = true;
                PlayRoar(-0.2f);
                break;
            case > 1020: // Move to pos
                NPC.MoveToPos(targetPosition, 0.2f, 0.2f, 0.4f, 0.4f);
                break;
            case > 900: // Slow down, move corrupt head downward
                NPC.velocity *= 0.93f;
                HeadCorrupt.Position = HeadCorrupt.DefaultPosition + Vector2.UnitY * 32;
                CursedFlamethrowerPrepDust();
                SetLegCrimsonFrame(LEG_ATTACK);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, -Pi / 6, 1 / 30f);
                if (AttackTimer == 960)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickPulse(NPC, HeadCorrupt.MiscPosition1, 2f, 15f, 5f, Color.GreenYellow);
                    }
                }
                GreenLaserEnabled = true;
                break;
            case 900: // Set flamethrower target
                if (LemonUtils.NotClient())
                {
                    AttackCount = Main.rand.NextBool().ToDirectionInt() * Pi / 2;
                }
                NPC.netUpdate = true;
                targetPosition = HeadCorrupt.MiscPosition1 + HeadCorrupt.MiscPosition1.DirectionTo(player.Center).RotatedBy(AttackCount) * 100;
                break;
            case > 720: // Flamethrower1 + Crimson leg swing
                GreenLaserEnabled = false;
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                NPC.velocity = Vector2.Zero;
                targetPosition = HeadCorrupt.MiscPosition1 + HeadCorrupt.MiscPosition1.DirectionTo(player.Center).RotatedBy(AttackCount) * 100;
                AttackCount *= 0.98f;
                if (AttackTimer % 10 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        CursedFlames(HeadCorrupt.MiscPosition1, HeadCorrupt.MiscPosition1.DirectionTo(targetPosition));
                    }
                }

                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, Pi / 4, 1 / 10f);
                if (LegCrimson.Rotation > 0 && AttackCount2 == 0)
                {
                    AttackCount2 = 1;
                    if (LemonUtils.NotClient())
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 pos = player.Center + LemonUtils.RandomVector2Circular(400, 400, 100, 100);
                            LemonUtils.QuickProj(
                                NPC,
                                LegCrimson.MiscPosition1,
                                Vector2.Zero,
                                ProjectileType<TinyMeatball>(),
                                ProjDamage,
                                ai0: pos.X,
                                ai1: pos.Y,
                                ai2: 60
                                );
                        }
                    }

                    NPC.netUpdate = true;
                }
                break;
            case > 540: // Swing leg back, move to center, reset leg
                if (LegCrimson.Rotation < 0 && AttackCount2 == 1)
                {
                    AttackCount2 = 2;
                    if (LemonUtils.NotClient())
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 pos = player.Center + LemonUtils.RandomVector2Circular(800, 800, 200, 200);
                            LemonUtils.QuickProj(
                                NPC,
                                LegCrimson.MiscPosition1,
                                Vector2.Zero,
                                ProjectileType<TinyMeatball>(),
                                ProjDamage,
                                ai0: pos.X,
                                ai1: pos.Y,
                                ai2: 60
                                );
                        }
                    }
                }
                CursedFlamethrowerPrepDust();
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                NPC.MoveToPos(arenaCenter, 0.2f, 0.05f, 0.2f, 0.2f);
                if (AttackTimer == 600)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickPulse(NPC, HeadCorrupt.MiscPosition1, 2f, 15f, 5f, Color.GreenYellow);
                    }
                }
                if (AttackTimer > 660)
                {
                    LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, -Pi / 4, 1 / 10f);
                }
                else
                {
                    SetLegCrimsonFrame(LEG_STANDARD);
                    LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, 0, 1 / 20f);
                }
                GreenLaserEnabled = true;
                break;
            case 540:
                if (LemonUtils.NotClient())
                {
                    AttackCount2 = Main.rand.NextBool().ToDirectionInt(); // Sign of which direction to rotate
                }
                NPC.netUpdate = true;
                AttackCount = AttackCount2 * (Pi / 4);

                targetPosition = player.Center;
                break;
            case > 300: // Slow down, second flamethrower
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                NPC.velocity *= 0.93f;
                Vector2 pos2 = HeadCorrupt.MiscPosition1 + HeadCorrupt.MiscPosition1.DirectionTo(targetPosition).RotatedBy(AttackCount) * 100;

                AttackCount -= AttackCount2 * (Pi / 4 / 120f);
                if (AttackTimer % 10 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        CursedFlames(HeadCorrupt.MiscPosition1, HeadCorrupt.MiscPosition1.DirectionTo(pos2), minSpeed: 100, maxSpeed: 120, duration: 30, turningAngle: Pi / 16);
                        CursedFlames(HeadCorrupt.MiscPosition1, -HeadCorrupt.MiscPosition1.DirectionTo(pos2), minSpeed: 100, maxSpeed: 120, duration: 30, turningAngle: Pi / 16);
                    }
                }
                GreenLaserEnabled = false;
                break;
            case > 180: // Move to center, ichor flamethrower
                if (AttackTimer % 10 == 0)
                {
                    LemonUtils.QuickPulse(
                        NPC,
                        HeadCrimson.Position,
                        3, 30, 5,
                        Color.Yellow
                        );
                }
                YellowLaserEnabled = true;
                targetPosition = player.Center;
                IchorFlamethrowerPrepDust();
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                NPC.MoveToPos(arenaCenter, 0.1f, 0.05f, 0.2f, 0.2f);
                break;
            case 180:
                LemonUtils.DustLine(HeadCrimson.MiscPosition1, targetPosition, DustID.GemTopaz, 16, 2f);
                LemonUtils.DustCircle(targetPosition, 8, 10, DustID.GemTopaz, 5f);
                break;
            case > 120:
                YellowLaserEnabled = false;
                NPC.MoveToPos(arenaCenter, 0.2f, 0.2f, 0.2f, 0.2f);
                break;
            case > 30:
                if (LemonUtils.NotClient())
                {
                    IchorFlames(HeadCrimson.MiscPosition1, HeadCrimson.MiscPosition1.DirectionTo(targetPosition), Pi / 4, 85, 100, 45, 0.97f, Pi / 32, 6);
                }
                NPC.velocity *= 0.93f;
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                break;
            case > 0:
                NPC.velocity = Vector2.Zero;
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                break;
            case 0:
                AttackTimer = 1080;
                return;
        }
        Main.NewText(AttackTimer);
        AttackTimer--;
    }

    void FlameWallsAndSpinning()
    {
        SetBodyPartPositions(headLerpSpeed: 0.9f, legLerpSpeed: 0.9f, bodyLerpSpeed: 0.9f);
        switch (AttackTimer)
        {
            case 1080:
                break;
            case > 990: // Move to center
                NPC.MoveToPos(arenaCenter, 0.1f, 0.1f, 0.25f, 0.25f);
                break;
            case 990: // Spawn circling flame spheres
                NPC.velocity = Vector2.Zero;
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                PlayRoar(-0.2f);
                if (LemonUtils.NotClient())
                {
                    CirclingFlameSpheres(8, 180, 10, 20);
                }
                break;
            case > 960: // Move to player
                EnableLasers(true);
                NPC.MoveToPos(player.Center, 0.05f, 0.05f, 0.1f, 0.1f);
                break;
            case 960: // Close corrupt mouth, Open crimson mouth
                EnableLasers(false);
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                PlayRoar(0.3f);
                break;
            case > 720: // Continue moving to player
                NPC.MoveToPos(player.Center, 0.05f, 0.05f, 0.2f, 0.2f);
                if (LemonUtils.NotClient() && AttackTimer % 20 == 0)
                {
                    LightningAroundPlayer(500, -1500, 120, 3000);
                }
                if (AttackTimer % 2 == 0)
                {
                    ShakeCrimsonHead(0, 12);
                }
                break;
            case > 660: // Slow down
                NPC.velocity *= 0.92f;
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                break;
            case 660:
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                PlayRoar(-0.2f);
                if (LemonUtils.NotClient())
                {
                    CirclingFlameSpheres(12, 240, 15, 25);
                }
                break;
            case > 600:
                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustDirect(player.Center + new Vector2(500, -1000 + 100 * i), 2, 2, DustID.CursedTorch, 0, -10, Scale: Main.rand.NextFloat(2, 4f)).noGravity = true;
                    Dust.NewDustDirect(player.Center + new Vector2(-500, -1000 + 100 * i), 2, 2, DustID.CursedTorch, 0, -10, Scale: Main.rand.NextFloat(2, 4f)).noGravity = true;
                }
                break;
            case 600:
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                targetPosition = player.Center;
                AttackCount = LemonUtils.Sign(targetPosition.X - NPC.Center.X, 1);
                if (LemonUtils.NotClient())
                {
                    PlaceCursedGeyser(
                        new Vector2(targetPosition.X + 500, targetPosition.Y + 1000),
                        -Vector2.UnitY * 100,
                        480,
                        10,
                        0
                        );
                    PlaceCursedGeyser(
                        new Vector2(targetPosition.X - 500, targetPosition.Y + 1000),
                        -Vector2.UnitY * 100,
                        480,
                        10,
                        0
                        );
                    if (LemonUtils.IsHard())
                    {
                        PlaceCursedGeyser(
                            new Vector2(targetPosition.X + 500, targetPosition.Y + 1000),
                            Vector2.UnitY * 100,
                            480,
                            10,
                            0
                            );
                        PlaceCursedGeyser(
                            new Vector2(targetPosition.X - 500, targetPosition.Y + 1000),
                            Vector2.UnitY * 100,
                            480,
                            10,
                            0
                            );
                    }

                }
                break;
            case > 120:
                Vector2 NPCmoveToPos = player.Center + AttackCount * Vector2.UnitX * 700;
                NPC.MoveToPos(NPCmoveToPos, 0.3f, 0.1f, 0.05f, 0.1f);
                break;
            case > 0:
                NPC.MoveToPos(arenaCenter, 0.3f, 0.3f, 0.3f, 0.3f);
                break;
            case 0:
                AttackTimer = 1080;
                return;
        }

        AttackTimer--;
    }

    void DashingWithBalls()
    {
        SetBodyPartPositions(headLerpSpeed: 0.9f, legLerpSpeed: 0.9f, bodyLerpSpeed: 0.9f);
        switch (AttackTimer)
        {
            case 1320: // Begin mini cutscene
                PlayRoar(-0.3f);
                LemonUtils.QuickScreenShake(NPC.Center, 60f, 8f, 120, 2000);
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                break;
            case > 1200: // Head shaking, pulses and dust
                SetBodyPartPositions(
                HeadCorrupt.DefaultPosition - Vector2.UnitY * 40 + Main.rand.NextVector2Circular(24, 24),
                HeadCrimson.DefaultPosition - Vector2.UnitY * 40 + Main.rand.NextVector2Circular(24, 24));
                if (AttackTimer % 10 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickPulse(NPC, NPC.Center, 2, 15, 5);
                    }
                    AttackCount++;
                }
                NPC.velocity *= 0.95f;
                AuraBurst((int)AttackCount, Vector2.UnitY * Main.rand.NextFloat(-20, -10));
                break;
            case 1200: // Activate shader, reset mouths
                shaderIsActive = true;
                ResetMouthFrames();
                break;
            case > 1170: // Prep leg
                SetLegCorruptFrame(LEG_ATTACK);
                break;
            case 1170: // Spawn giant cursed sphere (explodes at 140), save current NPC pos
                if (LemonUtils.NotClient())
                {
                    CirclingFlameSpheres(8, 120, 10, 20);
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.02f, 1170 - 140);
                    LemonUtils.QuickPulse(NPC, HeadCorrupt.MiscPosition1, 2, 15, 5, Color.GreenYellow);
                }
                SetLegCorruptFrame(LEG_STANDARD);
                targetPosition = NPC.Center;
                break;
            case > 1110: // Start moving away from saved pos (charging up)
                Vector2 awayPos = targetPosition + -NPCToPlayer * 320;
                NPC.MoveToPos(awayPos, 0.1f, 0.05f, 0.35f, 0.35f);
                CursedFlamethrowerPrepDust();
                EnableLasers(true);
                break;
            case 1110: // Dash to player
                EnableLasers(false);
                NPC.velocity = NPCToPlayer * 40;
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                AuraBurst(100, -NPCToPlayer.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10, 20));
                PlayRoar(-0.3f);
                break;
            case > 1080: // Flamethrower
                if (LemonUtils.NotClient() && AITimer % 5 == 0)
                {
                    CursedFlames(HeadCorrupt.MiscPosition1, -NPC.velocity.SafeNormalize(Vector2.Zero));
                }
                break;
            case > 1020: // Start slowing down, flamethrower
                if (LemonUtils.NotClient() && AITimer % 5 == 0)
                {
                    CursedFlames(HeadCorrupt.MiscPosition1, -NPC.velocity.SafeNormalize(Vector2.Zero));
                }
                NPC.velocity *= 0.95f;
                break;
            case > 990: // Full stop, prep leg for gcs
                NPC.velocity = Vector2.Zero;
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                SetLegCorruptFrame(LEG_ATTACK);
                break;
            case 990: // Spawn giant cursed sphere (explodes at 145), save current NPC pos
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.02f, 990 - 145);
                }
                SetLegCorruptFrame(LEG_STANDARD);
                targetPosition = NPC.Center;
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickPulse(NPC, HeadCorrupt.MiscPosition1, 2, 15, 5, Color.GreenYellow);
                }
                break;
            case > 930: // Start moving away again
                EnableLasers(true);
                Vector2 awayPos2 = targetPosition + -NPCToPlayer * 320;
                NPC.MoveToPos(awayPos2, 0.1f, 0.05f, 0.5f, 0.5f);
                break;
            case 930: // Dash
                EnableLasers(false);
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                NPC.velocity = NPCToPlayer * 40;
                AuraBurst(100, -NPCToPlayer.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10, 20));
                PlayRoar(-0.3f);
                break;
            case > 900: // Flamethrower
                NPC.velocity *= 0.99f;
                if (LemonUtils.NotClient() && AITimer % 5 == 0)
                {
                    CursedFlames(HeadCorrupt.MiscPosition1, -NPC.velocity.SafeNormalize(Vector2.Zero));
                }
                break;
            case 900: // Prep for lightning + cursed flame burst
                SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                PlayRoar(0.3f);
                for (int i = 0; i < 16; i++)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(
                            NPC,
                            HeadCorrupt.Position,
                            HeadCorrupt.Position.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * Main.rand.NextFloat(3, 6),
                            ProjectileType<CursedFlameSphere>(),
                            ProjDamage,
                            ai1: Main.rand.NextFloat(1.005f, 1.025f)
                            );
                    }
                }
                break;
            case > 750: // Lightning, move towards player
                if (AttackTimer % 15 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(600, -1500, 90, 3000);
                }
                NPC.MoveToPos(player.Center, 0.2f, 0.2f, 0.1f, 0.1f);
                HeadCrimson.Position = HeadCrimson.DefaultPosition - Vector2.UnitY * 32 + Main.rand.NextVector2Circular(12, 12);
                SetLegCorruptFrame(LEG_ATTACK);
                break;
            case 750: // Stop lightning, slow down
                NPC.velocity *= 0.93f;
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                break;
            case > 720: // Slowing down...
                NPC.velocity *= 0.93f;
                break;
            case 720: // Spawn giant cursed sphere (explodes at 120) and lost souls at head
                NPC.velocity *= 0.93f;
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.02f, 720 - 120);
                    for (int i = 0; i < 4; i++)
                    {
                        CrimsonLostSouls(HeadCrimson.Position, Vector2.UnitY.RotatedByRandom(Pi * 2) * 4, 60, 180);
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        if (LemonUtils.NotClient())
                        {
                            LemonUtils.QuickProj(
                                NPC,
                                HeadCorrupt.Position,
                                HeadCorrupt.Position.DirectionTo(player.Center).RotatedBy(Main.rand.NextFloat(-Pi / 4, Pi / 4)) * Main.rand.NextFloat(3, 6),
                                ProjectileType<CursedFlameSphere>(),
                                ProjDamage,
                                ai1: Main.rand.NextFloat(1.005f, 1.025f)
                                );
                        }
                    }
                }
                SetLegCorruptFrame(LEG_STANDARD);
                break;
            case > 690: // Slowing down idk why anymore
                NPC.velocity *= 0.93f;
                break;
            case 690: // Roar
                PlayRoar(-0.3f);
                break;
            case > 600:
                break;
            case 600: // Prep for lightning
                PlayRoar(0.4f);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                break;
            case > 420: // Spiraling dust, lightning around player
                AttackCount++;

                if (AttackTimer % 20 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickPulse(NPC, arenaCenter, 0.5f, 10, 5, Color.Gold);
                    }
                }
                // Teleport indicator
                float angle = ToRadians(45 - AttackCount);
                float distance = 1200 - AttackCount * 10;
                if (distance < 0) distance = 0;
                Vector2 dustOffset = Vector2.UnitY.RotatedBy(angle) * distance;
                if (AttackTimer % 2 == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 dustPos = arenaCenter + dustOffset.RotatedBy(PiOver2 * i);
                        Dust.NewDustPerfect(dustPos, DustID.GemTopaz, Vector2.Zero, Scale: 3f).noGravity = true;
                    }
                }
                NPC.Opacity -= 1 / 90f;

                if (AttackTimer % 20 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(600, -1500, 90, 3000);
                }
                HeadCrimson.Position = HeadCrimson.DefaultPosition - Vector2.UnitY * 32 + Main.rand.NextVector2Circular(12, 12);
                NPC.velocity *= 0.97f;
                break;
            case 420: // Teleport to arena center
                NPC.velocity = Vector2.Zero;
                AuraBurst(100, Vector2.UnitY * Main.rand.NextFloat(-20, -10));
                NPC.Center = arenaCenter;
                SetBodyPartPositions(headLerpSpeed: 1f, legLerpSpeed: 1f, bodyLerpSpeed: 1f);
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { PitchRange = (0.5f, 0.8f) }, NPC.Center);
                AuraBurst(100, Vector2.UnitY * Main.rand.NextFloat(-20, -10));
                break;
            case > 360: // More lightning, rotate legs inward
                NPC.Opacity += 1 / 10f;
                if (AttackTimer % 10 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(600, -1500, 90, 3000);
                }
                HeadCrimson.Position = HeadCrimson.DefaultPosition - Vector2.UnitY * 32 + Main.rand.NextVector2Circular(12, 12);
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, -ToRadians(20), 1 / 30f);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, ToRadians(20), 1 / 30f);
                SetLegCorruptFrame(LEG_ATTACK);
                SetLegCrimsonFrame(LEG_ATTACK);
                break;
            case 360: // Spawn several giant cursed spheres, exploding at 160 +- 10
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                if (LemonUtils.NotClient())
                {
                    Vector2 ballPos = Body.Position + Vector2.UnitY * (Body.Height / 2);
                    GiantCursedSphere(ballPos, 1.02f, 360 - 150, ToRadians(22.5f));
                    GiantCursedSphere(ballPos, 1.02f, 360 - 160, ToRadians(22.5f + 11.25f));
                    GiantCursedSphere(ballPos, 1.02f, 360 - 170, ToRadians(22.5f + 6.12f));
                }
                break;
            case > 160: // Move legs outward
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, ToRadians(30), 1 / 30f);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, -ToRadians(30), 1 / 30f);
                break;
            case > 150: // Move legs in quickly
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, -ToRadians(45), 1 / 5f);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, ToRadians(45), 1 / 5f);
                break;
            case 150: // Spheres explode, deactivate shader, roar, screen shake
                PlayRoar(0.3f);
                PlayRoar(-0.3f);
                shaderIsActive = false;
                AuraBurst(100, Vector2.UnitY * Main.rand.NextFloat(-20, -10));
                LemonUtils.QuickScreenShake(NPC.Center, 60f, 8f, 120, 2000);
                break;
            case > 120:
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, -ToRadians(45), 1 / 5f);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, ToRadians(45), 1 / 5f);
                break;
            case > 0: // Return legs to normal position
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, 0, 1 / 30f);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, 0, 1 / 30f);
                break;
            case 0:
                AttackTimer = 1320;
                return;
        }

        AttackTimer--;
    }

    void SwitchAttacks()
    {
        Attack++;
        Attack = 2;
        attackDuration = attackDurations[(int)Attack];
        targetLeg = null;
        AttackCount = 0;
        AttackCount2 = 0;
        AttackTimer = 0;
        shaderIsActive = false;
        GreenLaserEnabled = false;
        YellowLaserEnabled = false;
        ResetLegFrames();
        ResetMouthFrames();
        NPC.Opacity = 1f;
        ResetLegFrames();
    }

    void LightningAroundPlayer(float range, int height, int warningDuration, int length)
    {
        Vector2 randPos = player.Center + new Vector2(Main.rand.NextFloat(-range, range), height);
        LemonUtils.QuickProj(
            NPC,
            randPos,
            Vector2.Zero,
            ProjectileType<LightningWarningProj>(),
            ProjDamage,
            ai1: warningDuration,
            ai2: length
            );
    }

    void GiantCursedSphere(Vector2 pos, float acceleration, int duration, float angle = Pi / 8)
    {
        LemonUtils.QuickProj(NPC, pos, Vector2.Zero, ProjectileType<GiantCursedFlameSphere>(), ProjDamage,
                    ai0: angle,
                    ai1: acceleration,
                    ai2: duration);
    }

    void GiantMeatball(Vector2 pos, float timeLeft, int lostSoulWaitTime, float lostSoulTimeLeft, float speed = 6)
    {
        LemonUtils.QuickProj(NPC, pos, Vector2.UnitY * speed, ProjectileType<GiantMeatball>(), ProjDamage,
                    ai0: timeLeft,
                    ai1: lostSoulWaitTime,
                    ai2: lostSoulTimeLeft);
    }

    void IchorFlamethrowerPrepDust()
    {
        Dust.NewDustDirect(
            HeadCrimson.Position - Vector2.UnitY * 16 + Main.rand.NextVector2Circular(HeadCrimson.Width * 0.5f, HeadCrimson.Height * 0.1f),
            2, 2,
            DustID.IchorTorch,
            0, Main.rand.NextFloat(-12f, -8f),
            Scale: Main.rand.NextFloat(1.5f, 3f)
            ).noGravity = true;
    }

    void CursedFlamethrowerPrepDust()
    {
        Dust.NewDustDirect(
            HeadCorrupt.Position - Vector2.UnitY * 16 + Main.rand.NextVector2Circular(HeadCorrupt.Width * 0.5f, HeadCorrupt.Height * 0.1f),
            2, 2,
            DustID.CursedTorch,
            0, Main.rand.NextFloat(-12f, -8f),
            Scale: Main.rand.NextFloat(1.5f, 3f)
            ).noGravity = true;
    }

    void PlaceCursedGeyser(Vector2 pos, Vector2 flameVelocity, int geyserDuration = 480, float randomFlameSpeedOffset = 10, float flameTurningAngle = Pi / 16)
    {
        LemonUtils.QuickProj(
                        NPC,
                        pos,
                        flameVelocity,
                        ProjectileType<CursedGeyser>(),
                        ai0: geyserDuration,
                        ai1: randomFlameSpeedOffset,
                        ai2: flameTurningAngle
                        );
    }

    void CursedFlames(Vector2 position, Vector2 direction, float angle = Pi / 16f, float minSpeed = 45, float maxSpeed = 55, float duration = 30, float slowDownRate = 0.97f, float turningAngle = Pi / 32, int iterations = 6)
    {
        for (int i = 0; i < iterations; i++)
        {
            LemonUtils.QuickProj(
                NPC,
                position,
                direction.RotatedBy(Main.rand.NextFloat(-angle, angle)) * Main.rand.NextFloat(minSpeed, maxSpeed),
                ProjectileType<CursedFlamethrower>(),
                ProjDamage,
                ai0: 30,
                ai1: 0.97f,
                ai2: Main.rand.NextFloat(-turningAngle, turningAngle)
                );
        }
    }

    void IchorFlames(Vector2 position, Vector2 direction, float angle = Pi / 16f, float minSpeed = 45, float maxSpeed = 55, float duration = 30, float slowDownRate = 0.97f, float turningAngle = Pi / 32, int iterations = 6)
    {
        for (int i = 0; i < iterations; i++)
        {
            LemonUtils.QuickProj(
                NPC,
                position,
                direction.RotatedBy(Main.rand.NextFloat(-angle, angle)) * Main.rand.NextFloat(minSpeed, maxSpeed),
                ProjectileType<IchorFlamethrower>(),
                ProjDamage,
                ai0: duration,
                ai1: slowDownRate,
                ai2: Main.rand.NextFloat(-turningAngle, turningAngle)
                );
        }
    }

    void CrimsonLostSouls(Vector2 pos, Vector2 velocity, int waitTime = 60, int duration = 360)
    {
        LemonUtils.QuickProj(
                            NPC,
                            pos,
                            velocity,
                            ProjectileType<CrimsonLostSoul>(),
                            ProjDamage,
                            ai0: waitTime,
                            ai1: duration
                            );
    }

    void CirclingFlameSpheres(int count, int baseWaitTime, int iterWaitTimeOffset, float speed)
    {
        for (int i = 0; i < count; i++)
        {
            float circlingOffsetAngle = 2 * i * Pi / count;
            float waitTime = baseWaitTime + i * iterWaitTimeOffset;
            LemonUtils.QuickProj(
                NPC,
                HeadCorrupt.Position,
                Vector2.UnitY * speed,
                ProjectileType<CirclingCursedFlameSphere>(),
                ProjDamage,
                ai0: NPC.whoAmI,
                ai1: circlingOffsetAngle,
                ai2: waitTime
                );
        }
    }

    void PlayRoar(float bonusPitch = 0f)
    {
        SoundEngine.PlaySound(SoundID.Roar with { Pitch = -1f + bonusPitch }, NPC.Center);
        SoundEngine.PlaySound(SoundID.NPCDeath62 with { Pitch = -0.5f + bonusPitch }, NPC.Center);
    }

    private void AuraBurst(int count, Vector2 speed)
    {
        for (int i = 0; i < count; i++)
        {
            Dust.NewDustDirect(NPC.RandomPos(200, 100), 2, 2, DustID.GemTopaz, speed.X, speed.Y, Scale: Main.rand.NextFloat(2f, 3f));
        }
    }

    void ShakeCrimsonHead(float height = 32, float intensity = 24)
    {
        HeadCrimson.Position = HeadCrimson.DefaultPosition - Vector2.UnitY * height + Main.rand.NextVector2Circular(intensity, intensity);
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


    public override bool CheckDead()
    {
        return true;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return true;
    }

    public override void OnKill()
    {

    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ) return;

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    public void SetDefaultBodyPartPositions()
    {
        Body.DefaultPosition = NPC.Center;
        Body.Origin = Body.Texture.Size() * 0.5f;
        Body.MiscPosition1 = Body.Position + new Vector2(-Body.Width * 0.25f, -Body.Height * 0.2f) * NPC.scale;
        Body.MiscPosition2 = Body.Position + new Vector2(+Body.Width * 0.25f, -Body.Height * 0.2f) * NPC.scale;
        Body.Opacity = NPC.Opacity;
        Body.Scale = NPC.scale;

        LegCorrupt.DefaultPosition = Body.Position + new Vector2(-Body.Width * 0.57f, -Body.Height * 0.1f) * NPC.scale;
        LegCorrupt.MiscPosition1 = LegCorrupt.Position + new Vector2(LegCorrupt.Width * 0.1f, LegCorrupt.Height * 0.7f).RotatedBy(LegCorrupt.Rotation) * NPC.scale;
        LegCorrupt.Origin = new Vector2(LegCorrupt.Width * 0.5f, LegCorrupt.Height * 0.17f);
        LegCorrupt.Frames = 2;
        LegCorrupt.Opacity = NPC.Opacity;
        LegCorrupt.Scale = NPC.scale;

        LegCrimson.DefaultPosition = Body.Position + new Vector2(Body.Width * 0.57f, -Body.Height * 0.1f) * NPC.scale;
        LegCrimson.MiscPosition1 = LegCrimson.Position + new Vector2(LegCrimson.Width * 0.1f, LegCrimson.Height * 0.7f).RotatedBy(LegCrimson.Rotation) * NPC.scale;
        LegCrimson.Origin = new Vector2(LegCrimson.Width * 0.5f, LegCrimson.Height * 0.17f);
        LegCrimson.Opacity = NPC.Opacity;
        LegCrimson.Scale = NPC.scale;
        LegCrimson.Frames = 2;
        LegCrimson.Scale = NPC.scale;

        BackLegs.DefaultPosition = Body.Position + new Vector2(0, BackLegs.Height * 0.25f) * NPC.scale;
        BackLegs.Origin = BackLegs.Texture.Size() * 0.5f;
        BackLegs.Opacity = NPC.Opacity;
        BackLegs.Scale = NPC.scale;

        HeadCorrupt.DefaultPosition = Body.MiscPosition1 + new Vector2(0, -50) * NPC.scale;
        HeadCorrupt.MiscPosition1 = HeadCorrupt.DefaultPosition + new Vector2(0, -64) * NPC.scale;
        HeadCorrupt.Origin = HeadCorrupt.Texture.Size() * 0.5f;
        HeadCorrupt.Frames = 2;
        HeadCorrupt.Opacity = NPC.Opacity;
        HeadCorrupt.Scale = NPC.scale;

        HeadCrimson.DefaultPosition = Body.MiscPosition2 + new Vector2(0, -50) * NPC.scale;
        HeadCrimson.MiscPosition1 = HeadCrimson.DefaultPosition + new Vector2(0, -64) * NPC.scale;
        HeadCrimson.Origin = HeadCrimson.Texture.Size() * 0.5f;
        HeadCrimson.Frames = 2;
        HeadCrimson.Opacity = NPC.Opacity;
        HeadCrimson.Scale = NPC.scale;

        WingCorrupt.DefaultPosition = Body.Position - new Vector2(0, Body.Height * 0.2f) * NPC.scale;
        WingCorrupt.Origin = new Vector2(WingCorrupt.Width, WingCorrupt.Height * 0.5f);
        WingCorrupt.Frames = 6;
        WingCorrupt.Opacity = NPC.Opacity;
        WingCorrupt.Scale = NPC.scale;

        WingCrimson.DefaultPosition = Body.Position - new Vector2(0, Body.Height * 0.2f) * NPC.scale;
        WingCrimson.Origin = new Vector2(0, WingCrimson.Height * 0.5f);
        WingCrimson.Frames = 6;
        WingCrimson.Opacity = NPC.Opacity;
        WingCrimson.Scale = NPC.scale;
    }

    public void SetBodyPartPositions(Vector2 headCorruptTargetPosition = default,
                                    Vector2 headCrimsonTargetPosition = default,
                                    Vector2 legCorruptTargetPosition = default,
                                    Vector2 legCrimsonTargetPosition = default,
                                    Vector2 bodyTargetPosition = default,
                                    float headLerpSpeed = 1 / 10f,
                                    float legLerpSpeed = 1 / 10f,
                                    float bodyLerpSpeed = 1 / 10f)
    {
        if (headCorruptTargetPosition == default)
        {
            headCorruptTargetPosition = HeadCorrupt.DefaultPosition;
        }

        if (headCrimsonTargetPosition == default)
        {
            headCrimsonTargetPosition = HeadCrimson.DefaultPosition;
        }

        if (legCorruptTargetPosition == default)
        {
            legCorruptTargetPosition = LegCorrupt.DefaultPosition;
        }

        if (legCrimsonTargetPosition == default)
        {
            legCrimsonTargetPosition = LegCrimson.DefaultPosition;
        }

        if (bodyTargetPosition == default)
        {
            bodyTargetPosition = Body.DefaultPosition;
        }

        Body.Position = Vector2.Lerp(Body.Position, bodyTargetPosition, bodyLerpSpeed);

        LegCorrupt.Position = Vector2.Lerp(LegCorrupt.Position, legCorruptTargetPosition, legLerpSpeed);
        LegCrimson.Position = Vector2.Lerp(LegCrimson.Position, legCrimsonTargetPosition, legLerpSpeed);

        BackLegs.Position = BackLegs.DefaultPosition;
        HeadCorrupt.Position = Vector2.Lerp(HeadCorrupt.Position, headCorruptTargetPosition, headLerpSpeed);
        HeadCrimson.Position = Vector2.Lerp(HeadCrimson.Position, headCrimsonTargetPosition, headLerpSpeed);

        WingCorrupt.Position = WingCorrupt.DefaultPosition;
        WingCrimson.Position = WingCrimson.DefaultPosition;

    }


    /// <summary>
    /// If opposite is true, returns the opposite leg of the side the player is on.
    /// </summary>
    /// <returns>LegCorrupt if playerSide == 1, LegCrimson if playerSide == -1</returns>
    DreadlordBodyPart GetLeg(bool opposite = true)
    {
        if (!opposite)
        {
            return playerSide == 1 ? LegCrimson : LegCorrupt;
        }
        return playerSide == 1 ? LegCorrupt : LegCrimson;
    }

    #region Animation Methods
    void ResetLegFrames()
    {
        SetLegCorruptFrame(LEG_STANDARD);
        SetLegCrimsonFrame(LEG_STANDARD);
    }

    void ResetMouthFrames()
    {
        SetHeadCorruptFrame(HEAD_MOUTH_CLOSED);
        SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
    }

    void SetLegCorruptFrame(int frame)
    {
        LegCorrupt.CurrentFrame = frame;
    }

    void SetLegCrimsonFrame(int frame)
    {
        LegCrimson.CurrentFrame = frame;
    }

    void SetHeadCorruptFrame(int frame)
    {
        HeadCorrupt.CurrentFrame = frame;
    }

    void SetHeadCrimsonFrame(int frame)
    {
        HeadCrimson.CurrentFrame = frame;
    }

    void AnimateWings(int frameDuration)
    {
        if (wingAnimTimer > frameDuration)
        {
            wingAnimTimer = 0;
            wingFrame++;
        }
        if (wingFrame >= 6)
        {
            wingFrame = 0;
        }

        WingCorrupt.CurrentFrame = wingFrame;
        WingCrimson.CurrentFrame = wingFrame;
        wingAnimTimer++;
    }
    #endregion

    #region Drawing

    void EnableLasers(bool enabled)
    {
        GreenLaserEnabled = enabled;
        YellowLaserEnabled = enabled;
    }

    void DrawLaserCorrupt()
    {
        LemonUtils.DrawLaser(HeadCorrupt.Position + new Vector2(-0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f), player.Center, 2, Color.GreenYellow);
        LemonUtils.DrawLaser(HeadCorrupt.Position + new Vector2(0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f), player.Center, 2, Color.GreenYellow);
    }

    void DrawLaserCrimson()
    {
        LemonUtils.DrawLaser(HeadCrimson.Position + new Vector2(-0.2f * HeadCrimson.Width, -HeadCrimson.Height * 0.43f), player.Center, 2, Color.Yellow);
        LemonUtils.DrawLaser(HeadCrimson.Position + new Vector2(0.2f * HeadCrimson.Width, -HeadCrimson.Height * 0.43f), player.Center, 2, Color.Yellow);
    }

    void DrawNeck(Vector2 neckBase, Vector2 destination, Asset<Texture2D> texture)
    {
        Vector2 baseToDestination = neckBase.DirectionTo(destination);
        float distanceLeft = neckBase.Distance(destination);
        float rotation = baseToDestination.ToRotation() - MathHelper.PiOver2;

        Vector2 drawPos = neckBase;

        while (distanceLeft > -texture.Height() * 0.9f * NPC.scale)
        {
            Main.EntitySpriteDraw(texture.Value,
                drawPos - Main.screenPosition,
                null,
                Color.White * NPC.Opacity,
                rotation,
                texture.Size() * 0.5f,
                NPC.scale,
                SpriteEffects.None);
            if (shaderIsActive)
            {
                var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
                shader.Shader.Parameters["uTime"].SetValue(AITimer);
                shader.Shader.Parameters["color"].SetValue(Color.Yellow.ToVector4() * NPC.Opacity);
                shader.Shader.Parameters["moveSpeed"].SetValue(2f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
                Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
                shader.Apply();
                Main.EntitySpriteDraw(texture.Value,
                                    drawPos - Main.screenPosition,
                                    null,
                                    Color.White * NPC.Opacity,
                                    rotation,
                                    texture.Size() * 0.5f,
                                    NPC.scale,
                                    SpriteEffects.None);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            drawPos += baseToDestination * texture.Height() * 0.9f * NPC.scale;
            distanceLeft -= texture.Height() * 0.9f * NPC.scale;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        BackLegs.Draw(shaderIsActive, (int)AITimer);
        WingCorrupt.Draw(shaderIsActive, (int)AITimer);
        WingCrimson.Draw(shaderIsActive, (int)AITimer);
        Body.Draw(shaderIsActive, (int)AITimer);
        LegCorrupt.Draw(shaderIsActive, (int)AITimer);
        LegCrimson.Draw(shaderIsActive, (int)AITimer);
        DrawNeck(Body.MiscPosition1, HeadCorrupt.Position, neckTextureCorrupt);
        DrawNeck(Body.MiscPosition2, HeadCrimson.Position, neckTextureCrimson);
        HeadCorrupt.Draw(shaderIsActive, (int)AITimer);
        HeadCrimson.Draw(shaderIsActive, (int)AITimer);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        //Eye glow
        if (HeadCorrupt.CurrentFrame == HEAD_MOUTH_CLOSED)
        {
            LemonUtils.DrawGlow(HeadCorrupt.Position + new Vector2(-0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f) * NPC.scale, Color.LightGreen * NPC.Opacity, 0.8f * NPC.scale, 1f);
            LemonUtils.DrawGlow(HeadCorrupt.Position + new Vector2(0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f) * NPC.scale, Color.LightGreen * NPC.Opacity, 0.8f * NPC.scale, 1f);
        }

        if (HeadCrimson.CurrentFrame == HEAD_MOUTH_CLOSED)
        {
            LemonUtils.DrawGlow(HeadCrimson.Position + new Vector2(0.2f * HeadCrimson.Width, -HeadCrimson.Height * 0.43f) * NPC.scale, Color.Yellow * NPC.Opacity, 0.8f * NPC.scale, 1f);
        }

        if (GreenLaserEnabled)
        {
            DrawLaserCorrupt();
        }

        if (YellowLaserEnabled)
        {
            DrawLaserCrimson();
        }
    }
    #endregion
}
