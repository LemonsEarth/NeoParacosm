using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Content.Projectiles.Hostile;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using NeoParacosm.Content.Projectiles.Hostile.Researcher;
using NeoParacosm.Core.Players.NPEffectPlayers;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.UI;
using NeoParacosm.Core.UI.ResearcherUI.Boss;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.RGB;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;
using static Microsoft.Xna.Framework.MathHelper;

namespace NeoParacosm.Content.NPCs.Bosses.Dreadlord;

[AutoloadBossHead]
public class Dreadlord : ModNPC
{
    #region Attack Fields and Data
    ref float AITimer => ref NPC.ai[0];
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
    ref float AttackTimer => ref NPC.ai[2];
    ref float AttackCount => ref NPC.ai[3];
    int playerSide = 1; // whether NPC is to the left (-1) or right (1) of the player
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

    float attackDuration = 0;
    int[] attackDurations = { 600, 900, 1320, 600, 600 };

    public enum Attacks
    {
        BallsNLightning,
        DashingWithBalls,
    }

    int wingFrame = 0;
    int wingAnimTimer = 0;
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
    float ProjDamage => NPC.damage / 2;

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
        NPC.lifeMax = 80000;
        NPC.defense = 40;
        NPC.damage = 40;
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
        AnimateWings(8);
        AITimer++;
    }

    void Intro()
    {
        //NPC.dontTakeDamage = true;

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

    void AttackControl()
    {
        switch (Attack)
        {
            case (int)Attacks.BallsNLightning:
                BallsNLightning();
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
            case 600:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    targetPosition = arenaCenter + Main.rand.NextVector2Circular(600, 600);
                }
                NPC.netUpdate = true;
                break;
            case > 450:
                NPC.MoveToPos(targetPosition, 0.04f, 0.04f, 0.1f, 0.1f);
                break;
            case > 420:
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, ToRadians(60), 1 / 10f); // Move leg outward
                SetLegCorruptFrame(LEG_ATTACK);
                NPC.velocity *= 0.95f;
                break;
            case 420:
                NPC.velocity = Vector2.Zero;
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.05f, 120);
                }
                PlayRoar(0.3f);
                AttackCount++;
                break;
            case > 270:
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                if (AttackTimer % 15 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(1200, -1500, 60, 3000);

                    LemonUtils.QuickProj(
                        NPC,
                        player.Center + new Vector2(Main.rand.NextFloat(-800, 800), 800),
                        -Vector2.UnitY * 5,
                        ProjectileType<CrimsonLostSoul>(),
                        ProjDamage,
                        ai0: 60,
                        ai1: 240
                        );

                }
                break;
            case 270:
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                SetLegCorruptFrame(LEG_STANDARD);
                AttackCount++;
                break;
            case > 240:
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, 0, 1 / 10f); // Move leg back
                break;
            case 240:
                SetLegCorruptFrame(LEG_ATTACK);
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.05f, 120);
                }
                break;
            case 210:
                PlayRoar(0.6f);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                AttackCount++;
                break;
            case > 120 and < 210:
                LegCorrupt.Rotation = 0;
                if (AttackTimer % 10 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(1200, -1500, 90, 3000);
                }
                break;
            case 120:
                SetLegCorruptFrame(LEG_STANDARD);
                break;
            case > 60 and < 210:
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

    void DashingWithBalls()
    {
        SetBodyPartPositions(headLerpSpeed: 0.9f, legLerpSpeed: 0.9f, bodyLerpSpeed: 0.9f);
        switch (AttackTimer)
        {
            case 1320:
                PlayRoar(-0.3f);
                LemonUtils.QuickScreenShake(NPC.Center, 60f, 8f, 120, 2000);
                SetHeadCorruptFrame(HEAD_MOUTH_OPEN);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                break;
            case > 1200:
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
                AuraBurst((int)AttackCount, Vector2.UnitY * Main.rand.NextFloat(-20, -10));
                break;
            case 1200:
                shaderIsActive = true;
                ResetMouthFrames();
                break;
            case > 1170:
                SetLegCorruptFrame(LEG_ATTACK);
                break;
            case 1170:
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.02f, 1170 - 140);
                }
                SetLegCorruptFrame(LEG_STANDARD);
                targetPosition = NPC.Center;
                break;
            case > 1110:
                Vector2 awayPos = targetPosition + -NPCToPlayer * 320;
                NPC.MoveToPos(awayPos, 0.1f, 0.1f, 0.35f, 0.35f);
                break;
            case 1110:
                NPC.velocity = NPCToPlayer * 40;
                AuraBurst(100, -NPCToPlayer.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10, 20));
                PlayRoar(-0.3f);
                break;
            case > 1080:
                break;
            case > 1020:
                NPC.velocity *= 0.95f;
                break;
            case > 990:
                NPC.velocity = Vector2.Zero;
                SetLegCorruptFrame(LEG_ATTACK);
                break;
            case 990:
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.02f, 990 - 145);
                }
                SetLegCorruptFrame(LEG_STANDARD);
                break;
            case > 930:
                Vector2 awayPos2 = targetPosition + -NPCToPlayer * 320;
                NPC.MoveToPos(awayPos2, 0.1f, 0.1f, 0.5f, 0.5f);
                break;
            case 930:
                NPC.velocity = NPCToPlayer * 30;
                AuraBurst(100, -NPCToPlayer.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10, 20));
                PlayRoar(-0.3f);
                break;
            case > 900:
                break;
            case 900:
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                PlayRoar(0.3f);
                break;
            case > 750:
                if (AttackTimer % 25 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(900, -1500, 90, 3000);
                }
                NPC.MoveToPos(player.Center, 0.1f, 0.1f, 0.3f, 0.3f);
                HeadCrimson.Position = HeadCrimson.DefaultPosition - Vector2.UnitY * 32 + Main.rand.NextVector2Circular(12, 12);
                SetLegCorruptFrame(LEG_ATTACK);
                break;
            case 750:
                NPC.velocity *= 0.93f;
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                break;
            case > 720:
                NPC.velocity *= 0.93f;
                break;
            case 720:
                NPC.velocity *= 0.93f;
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                if (LemonUtils.NotClient())
                {
                    GiantCursedSphere(LegCorrupt.MiscPosition1, 1.02f, 120);
                    for (int i = 0; i < 7; i++)
                    {
                        CrimsonLostSouls(HeadCrimson.Position, Vector2.UnitY.RotatedByRandom(Pi * 2) * 5, 60, 240);
                    }
                }
                SetLegCorruptFrame(LEG_STANDARD);
                break;
            case > 690:
                NPC.velocity *= 0.93f;
                break;
            case 690:
                PlayRoar(-0.3f);
                break;
            case > 600:
                break;
            case 600:
                PlayRoar(0.4f);
                SetHeadCrimsonFrame(HEAD_MOUTH_OPEN);
                break;
            case > 420:
                AttackCount++;

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

                if (AttackTimer % 20 == 0 && LemonUtils.NotClient())
                {
                    LightningAroundPlayer(600, -1500, 90, 3000);
                }
                HeadCrimson.Position = HeadCrimson.DefaultPosition - Vector2.UnitY * 32 + Main.rand.NextVector2Circular(12, 12);
                NPC.velocity *= 0.97f;
                break;
            case 420:
                NPC.velocity = Vector2.Zero;
                AuraBurst(100, Vector2.UnitY * Main.rand.NextFloat(-20, -10));
                NPC.Center = arenaCenter;
                SetBodyPartPositions(headLerpSpeed: 1f, legLerpSpeed: 1f, bodyLerpSpeed: 1f);
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { PitchRange = (0.5f, 0.8f) }, NPC.Center);
                AuraBurst(100, Vector2.UnitY * Main.rand.NextFloat(-20, -10));
                break;
            case > 360:
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
            case 360:
                SetHeadCrimsonFrame(HEAD_MOUTH_CLOSED);
                if (LemonUtils.NotClient())
                {
                    Vector2 ballPos = Body.Position + Vector2.UnitY * (Body.Height / 2);
                    GiantCursedSphere(ballPos, 1.02f, 360 - 150, ToRadians(22.5f));
                    GiantCursedSphere(ballPos, 1.02f, 360 - 160, ToRadians(22.5f + 11.25f));
                    GiantCursedSphere(ballPos, 1.02f, 360 - 170, ToRadians(22.5f + 6.12f));
                }
                break;
            case > 160:
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, ToRadians(30), 1 / 30f);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, -ToRadians(30), 1 / 30f);
                break;
            case > 150:
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, -ToRadians(45), 1 / 5f);
                LegCrimson.Rotation = Utils.AngleLerp(LegCrimson.Rotation, ToRadians(45), 1 / 5f);
                break;
            case 150:
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
            case > 0:
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
        attackDuration = attackDurations[(int)Attack];
        targetLeg = null;
        AttackCount = 0;
        AttackTimer = 0;
        shaderIsActive = false;
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
        Body.MiscPosition1 = Body.Position + new Vector2(-Body.Width * 0.25f, -Body.Height * 0.2f);
        Body.MiscPosition2 = Body.Position + new Vector2(+Body.Width * 0.25f, -Body.Height * 0.2f);

        LegCorrupt.DefaultPosition = Body.Position + new Vector2(-Body.Width * 0.57f, -Body.Height * 0.1f);
        LegCorrupt.MiscPosition1 = LegCorrupt.Position + new Vector2(LegCorrupt.Width * 0.1f, LegCorrupt.Height * 0.7f).RotatedBy(LegCorrupt.Rotation);
        LegCorrupt.Origin = new Vector2(LegCorrupt.Width * 0.5f, LegCorrupt.Height * 0.17f);
        LegCorrupt.Frames = 2;

        LegCrimson.DefaultPosition = Body.Position + new Vector2(Body.Width * 0.57f, -Body.Height * 0.1f);
        LegCrimson.MiscPosition1 = LegCrimson.Position + new Vector2(LegCrimson.Width * 0.1f, LegCrimson.Height * 0.7f).RotatedBy(LegCrimson.Rotation);

        LegCrimson.Origin = new Vector2(LegCrimson.Width * 0.5f, LegCrimson.Height * 0.17f);
        LegCrimson.Frames = 2;

        BackLegs.DefaultPosition = Body.Position + new Vector2(0, BackLegs.Height * 0.25f);
        BackLegs.Origin = BackLegs.Texture.Size() * 0.5f;

        HeadCorrupt.DefaultPosition = Body.MiscPosition1 + new Vector2(0, -50);
        HeadCorrupt.Origin = HeadCorrupt.Texture.Size() * 0.5f;
        HeadCorrupt.Frames = 2;

        HeadCrimson.DefaultPosition = Body.MiscPosition2 + new Vector2(0, -50);
        HeadCrimson.Origin = HeadCrimson.Texture.Size() * 0.5f;
        HeadCrimson.Frames = 2;

        WingCorrupt.DefaultPosition = Body.Position - new Vector2(0, Body.Height * 0.2f);
        WingCorrupt.Origin = new Vector2(WingCorrupt.Width, WingCorrupt.Height * 0.5f);
        WingCorrupt.Frames = 6;

        WingCrimson.DefaultPosition = Body.Position - new Vector2(0, Body.Height * 0.2f);
        WingCrimson.Origin = new Vector2(0, WingCrimson.Height * 0.5f);
        WingCrimson.Frames = 6;

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

    void DrawNeck(Vector2 neckBase, Vector2 destination, Asset<Texture2D> texture)
    {
        Vector2 baseToDestination = neckBase.DirectionTo(destination);
        float distanceLeft = neckBase.Distance(destination);
        float rotation = baseToDestination.ToRotation() - MathHelper.PiOver2;

        Vector2 drawPos = neckBase;

        while (distanceLeft > -texture.Height() * 0.9f)
        {
            Main.EntitySpriteDraw(texture.Value,
                drawPos - Main.screenPosition,
                null,
                Color.White,
                rotation,
                texture.Size() * 0.5f,
                NPC.scale,
                SpriteEffects.None);
            if (shaderIsActive)
            {
                var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
                shader.Shader.Parameters["uTime"].SetValue(AITimer);
                shader.Shader.Parameters["color"].SetValue(Color.Yellow.ToVector4());
                shader.Shader.Parameters["moveSpeed"].SetValue(2f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
                Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
                shader.Apply();
                Main.EntitySpriteDraw(texture.Value,
                                    drawPos - Main.screenPosition,
                                    null,
                                    Color.White,
                                    rotation,
                                    texture.Size() * 0.5f,
                                    NPC.scale,
                                    SpriteEffects.None);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            drawPos += baseToDestination * texture.Height() * 0.9f;
            distanceLeft -= texture.Height() * 0.9f;
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
        if (HeadCorrupt.CurrentFrame == HEAD_MOUTH_CLOSED)
        {
            LemonUtils.DrawGlow(HeadCorrupt.Position + new Vector2(-0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f), Color.LightGreen, 0.8f, 1f);
            LemonUtils.DrawGlow(HeadCorrupt.Position + new Vector2(0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f), Color.LightGreen, 0.8f, 1f);
        }

        if (HeadCrimson.CurrentFrame == HEAD_MOUTH_CLOSED)
        {
            LemonUtils.DrawGlow(HeadCrimson.Position + new Vector2(0.2f * HeadCrimson.Width, -HeadCrimson.Height * 0.43f), Color.Yellow, 0.8f, 1f);
        }
    }
    #endregion
}
