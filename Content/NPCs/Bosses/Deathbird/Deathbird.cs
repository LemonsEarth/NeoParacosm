using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Items.BossBags;
using NeoParacosm.Content.Items.Placeable.Relics;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Content.Items.Weapons.Summon;
using NeoParacosm.Content.NPCs.Hostile.Minions;
using NeoParacosm.Content.Projectiles.Hostile;
using NeoParacosm.Content.Projectiles.None;
using NeoParacosm.Core.Systems;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;
using static Terraria.ModLoader.ModContent;

namespace NeoParacosm.Content.NPCs.Bosses.Deathbird;

[AutoloadBossHead]
public class Deathbird : ModNPC
{
    static Asset<Texture2D> headTexture;
    static Asset<Texture2D> bodyTexture;
    static Asset<Texture2D> leftLeg1Texture;
    static Asset<Texture2D> leftLeg2Texture;
    static Asset<Texture2D> rightLeg1Texture;
    static Asset<Texture2D> rightLeg2Texture;
    static Asset<Texture2D> wingsRightTexture;
    static Asset<Texture2D> wingsLeftTexture;

    struct BodyPart
    {
        public Vector2 pos = Vector2.Zero;
        public Vector2 origin = Vector2.Zero;
        public float rot = 0;

        public BodyPart(Vector2 pos, Vector2 origin, float rot)
        {
            this.pos = pos;
            this.origin = origin;
            this.rot = rot;
        }
    }

    BodyPart body = new BodyPart();
    BodyPart head = new BodyPart();
    BodyPart leftLeg1 = new BodyPart();
    BodyPart leftLeg2 = new BodyPart();
    BodyPart rightLeg1 = new BodyPart();
    BodyPart rightLeg2 = new BodyPart();

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
            int maxVal = phase == 1 ? 3 : 3;
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

    bool phase2Reached = false;
    int phase = 1;
    bool phaseTransition = false;

    float attackDuration = 0;
    int[] attackDurations = { 990, 900, 720, 600 };
    int[] attackDurations2 = { 1200, 720, 1200, 900, 900, 1080, 960 };
    public Player player { get; private set; }
    Vector2 targetPosition = Vector2.Zero;

    float projDamage => NPC.damage / 2;

    float wingOutlineScale = 1f;
    float wingScale = 0.9f;
    float darkColorBoost = 0f;

    public enum Attacks
    {
        HomingDeathflameBalls,
        HoverLingeringFlame,
        LaserFeathers,
        Grab,
    }

    public enum Attacks2
    {
        FeatherRain,
        LingeringFlameRain,
        ArenaDeathflame,
        DeathflameKamikaze,
    }

    public override void Load()
    {
        headTexture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/DeathbirdHead");
        bodyTexture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/DeathbirdBody");
        wingsLeftTexture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/DeathbirdWingsLeft");
        wingsRightTexture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/DeathbirdWingsRight");
        leftLeg1Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/DeathbirdLegLeft1");
        leftLeg2Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/DeathbirdLegLeft2");
        rightLeg1Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/DeathbirdLegRight1");
        rightLeg2Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/DeathbirdLegRight2");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
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
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Bestiary")),
            });
    }

    public override void SetDefaults()
    {
        NPC.width = 80;
        NPC.height = 80;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.Opacity = 1;
        NPC.lifeMax = 8000;
        NPC.defense = 10;
        NPC.damage = 40;
        NPC.HitSound = SoundID.DD2_SkeletonHurt;
        NPC.DeathSound = SoundID.NPCDeath52;
        NPC.value = 150000;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 10;
        NPC.SpawnWithHigherTime(30);

        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Common/Assets/Audio/Music/Gravefield");
        }
    }

    int frameDuration = 6;
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += 1;
        if (NPC.frameCounter > frameDuration)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 4 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!DownedBossSystem.downedDeathbird && spawnInfo.Player.InModBiome<DeadForestBiome>() && !NPC.AnyNPCs(NPCType<Deathbird>()))
        {
            return 1f;
        }
        return 0f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.5f);
    }

    int IntroDuration = 300;
    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];
        SetBodyPartPositions();

        DespawnCheck();
        if (AITimer < IntroDuration)
        {
            Intro();
            AITimer++;
            return;
        }
        if (!phaseTransition) NPC.dontTakeDamage = false;

        if (NPC.life < NPC.lifeMax / 2 && !phase2Reached)
        {
            phaseTransition = true;
            phase2Reached = true;
            AttackTimer = 0;
        }

        if (phaseTransition)
        {
            PhaseTransition();
            AITimer++;
            return;
        }

        if (phase == 1)
        {
            switch (Attack)
            {
                case (int)Attacks.HomingDeathflameBalls:
                    HomingDeathfireBalls();
                    break;
                case (int)Attacks.HoverLingeringFlame:
                    HoverLingeringFlame();
                    break;
                case (int)Attacks.LaserFeathers:
                    LaserFeathers();
                    break;
                case (int)Attacks.Grab:
                    Grab();
                    break;
            }
        }
        else if (phase == 2)
        {
            darkColorBoost = (float)Math.Sin(AITimer / 20f) * 0.5f + 1f;
            switch (Attack)
            {
                case (int)Attacks2.FeatherRain:
                    FeatherRain();
                    break;
                case (int)Attacks2.LingeringFlameRain:
                    LingeringFlameRain();
                    break;
                case (int)Attacks2.ArenaDeathflame:
                    ArenaDeathflame();
                    break;
                case (int)Attacks2.DeathflameKamikaze:
                    DeathflameKamikaze();
                    break;
            }
        }

        attackDuration--;
        if (attackDuration <= 0)
        {
            SwitchAttacks();
        }

        AITimer++;
    }

    void DefaultBody()
    {
        body.pos = NPC.Center;
        body.origin = bodyTexture.Size() * 0.5f;
        body.rot = NPC.rotation;
    }
    void DefaultHead()
    {
        head.pos = body.pos - Vector2.UnitY.RotatedBy(NPC.rotation) * bodyTexture.Height() * 0.5f;
        head.origin = headTexture.Size() * 0.5f;
        head.rot = NPC.rotation;
    }
    void DefaultLeftLeg1()
    {
        leftLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(NPC.rotation) * bodyTexture.Height() * 0.4f;
        leftLeg1.origin = new Vector2(leftLeg1Texture.Width(), 0);
        leftLeg1.rot = NPC.rotation;
    }
    void DefaultLeftLeg2()
    {
        leftLeg2.pos = leftLeg1.pos + new Vector2(-leftLeg1Texture.Width() * 0.5f, leftLeg1Texture.Height() * 0.8f).RotatedBy(leftLeg1.rot);
        leftLeg2.origin = new Vector2(leftLeg2Texture.Width(), 0);
        leftLeg2.rot = NPC.rotation;
    }
    void DefaultRightLeg1()
    {
        rightLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(NPC.rotation) * bodyTexture.Height() * 0.4f;
        rightLeg1.origin = Vector2.Zero;
        rightLeg1.rot = NPC.rotation;
    }
    void DefaultRightLeg2()
    {
        rightLeg2.pos = rightLeg1.pos + new Vector2(rightLeg1Texture.Width() * 0.5f, rightLeg1Texture.Height() * 0.8f).RotatedBy(rightLeg1.rot);
        rightLeg2.origin = Vector2.Zero;
        rightLeg2.rot = NPC.rotation;
    }
    void SetDefaultBodyPartValues()
    {
        DefaultBody();
        DefaultHead();
        DefaultLeftLeg1();
        DefaultLeftLeg2();
        DefaultRightLeg1();
        DefaultRightLeg2();
    }
    void BasicMovementAnimation()
    {
        float headRot = MathHelper.Clamp(NPC.velocity.X * 6, -90, 90);
        float bodyRot = MathHelper.Clamp(NPC.velocity.X * 3, -60, 60);
        float legRot = MathHelper.Clamp(NPC.velocity.X * 10, -75, 75);
        float leg2Rot = MathHelper.Clamp(NPC.velocity.X * 10, -90, 90);
        head.rot = Utils.AngleLerp(head.rot, MathHelper.ToRadians(headRot), 1 / 20f);
        body.rot = Utils.AngleLerp(body.rot, MathHelper.ToRadians(bodyRot), 1 / 20f);

        leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, MathHelper.ToRadians(legRot), 1 / 20f);
        leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, MathHelper.ToRadians(leg2Rot), 1 / 10f);

        rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, MathHelper.ToRadians(legRot), 1 / 20f);
        rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, MathHelper.ToRadians(leg2Rot), 1 / 10f);
    }

    void SwitchAttacks()
    {
        PlayRoar();
        Terraria.Graphics.Effects.Filters.Scene.Deactivate("NeoParacosm:DeathbirdArenaShader");

        Attack++;
        if (phase == 1) attackDuration = attackDurations[(int)Attack];
        else attackDuration = attackDurations2[(int)Attack];

        AttackCount = 0;
        AttackTimer = 0;
        NPC.Opacity = 1f;

        drawClone = false;
        clonePos = Vector2.Zero;
        cloneOpacity = 0f;
        cloneScale = 1f;

        arenaProgressTimer = 0;
    }

    const int HomingDeathfireBallsDuration = 330;
    const int HomingDeathfireBallsMoveTime = 240;
    const int HomingDeathfireBallsFireTime = 150;
    void HomingDeathfireBalls()
    {
        BasicMovementAnimation();
        switch (AttackTimer)
        {
            case HomingDeathfireBallsDuration:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    targetPosition = NPC.Center + new Vector2(Main.rand.Next(-250, 250), Main.rand.Next(-250, 250));
                }
                AttackCount++;
                NPC.netUpdate = true;
                break;
            case > HomingDeathfireBallsMoveTime:
                frameDuration = 6;
                NPC.MoveToPos(targetPosition, 0.3f, 0.3f, 0.2f, 0.2f);
                break;
            case > HomingDeathfireBallsFireTime:
                frameDuration = 24;
                NPC.velocity.X *= 0.95f;
                NPC.velocity.Y += 0.1f;
                if (AttackTimer % 60 == 0)
                {
                    NPC.velocity.Y -= 5;
                    SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned with { PitchRange = (0f, 0.2f) }, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ProjectileType<SuckyProjectile>(), projDamage, ai0: 1500, ai1: 50, ai2: 0);
                    }
                }
                break;
            case HomingDeathfireBallsFireTime:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 3 * LemonUtils.GetDifficulty(); i++)
                    {
                        Vector2 velocity = Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(2, 5);
                        LemonUtils.QuickProj(NPC, NPC.RandomPos(64, 64), velocity, ProjectileType<DeathflameBall>(), projDamage, ai0: 60, ai1: NPC.target);
                    }
                }
                break;
            case > HomingDeathfireBallsFireTime - 90:
                NPC.velocity *= 0.98f;
                NPC.velocity.Y += 0.05f;
                frameDuration = 999;
                break;
            case > 0:
                if (attackDuration < 60)
                {
                    // This attack repeats multiple times before attackDuration is up, so we check attackDuration 
                    // to make sure its the last rep
                    DrawClone(player.Center - Vector2.UnitY * 300, 0.01f, 2f);
                }
                frameDuration = 6;
                break;
            case 0:
                AttackTimer = HomingDeathfireBallsDuration;
                return;
        }
        AttackTimer--;
    }

    const int HoverLingeringFlameDuration = 300;
    const int HoverLingeringFlameInterval = 20;
    void HoverLingeringFlame()
    {
        BasicMovementAnimation();
        frameDuration = 6;
        switch (AttackTimer)
        {
            case HoverLingeringFlameDuration:
                TeleportToPos(player.Center - Vector2.UnitY * 300);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = -8; i <= 8; i++)
                    {
                        Vector2 pos = NPC.Center + Vector2.UnitX * i * 100;
                        LemonUtils.QuickProj(NPC, pos, new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(2, 4)), ProjectileType<LingeringDeathflame>(), projDamage, ai0: player.whoAmI, ai1: 420);
                    }
                }
                break;
            case > HoverLingeringFlameDuration - 60:
                NPC.velocity = Vector2.Zero;
                break;
            case > 0:
                NPC.MoveToPos(player.Center - Vector2.UnitY * 300, 0.2f, 0.2f, 0.2f, 0.2f);
                if (AttackTimer % HoverLingeringFlameInterval == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            LemonUtils.QuickProj(NPC, NPC.RandomPos(), Vector2.UnitY * 3, ProjectileType<LingeringDeathflame>(), projDamage, ai0: player.whoAmI, ai1: 360);
                        }
                    }
                }
                break;
            case 0:
                AttackTimer = HoverLingeringFlameDuration;
                return;
        }
        AttackTimer--;
    }

    const int LaserFeathersDuration = 240;
    const int LaserFeathersMoveTime = 120;
    void LaserFeathers()
    {
        BasicMovementAnimation();
        switch (AttackTimer)
        {
            case LaserFeathersDuration:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    targetPosition = NPC.Center + Main.rand.NextVector2Circular(400, 400);
                }
                NPC.netUpdate = true;
                break;
            case > LaserFeathersMoveTime:
                frameDuration = 12;
                NPC.MoveToPos(targetPosition, 0.2f, 0.2f, 0.1f, 0.1f);
                break;
            case > 0:
                if (attackDuration < 120)
                {
                    DrawClone(player.Center - Vector2.UnitY * 300, 0.005f, 2f);
                }
                NPC.velocity = Vector2.Zero;
                frameDuration = 9999;
                int cd = 20 - ((LemonUtils.GetDifficulty() - 1) * 5);
                if (AttackTimer % cd == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 pos = NPC.Center + new Vector2(16 * i, -8).RotatedBy(AttackCount * MathHelper.ToRadians(50 + AttackCount) * i);
                            Vector2 dir = NPC.Center.DirectionTo(pos);
                            int timeToFire = 60 - (LemonUtils.GetDifficulty() * 5);
                            LemonUtils.QuickProj(NPC, pos, dir * 10, ProjectileType<DeathbirdFeather>(), projDamage, ai0: timeToFire);
                        }
                    }
                    AttackCount++;
                }
                break;
            case 0:
                AttackTimer = LaserFeathersDuration;
                AttackCount = 0;
                return;
        }

        AttackTimer--;
    }

    int grabProjIdentity = -1;

    const int GrabDuration = 600;
    const int GrabPrepTime = 480;
    const int GrabEndTime = 450;
    Vector2 grabOffset => Vector2.UnitY.RotatedBy(body.rot - MathHelper.PiOver2) * bodyTexture.Height();
    void Grab()
    {
        frameDuration = 6;
        switch (AttackTimer)
        {
            case GrabDuration:
                SetDefaultBodyPartValues();
                TeleportToPos(player.Center - Vector2.UnitY * 300);
                break;
            case > GrabPrepTime:
                targetPosition = player.DirectionTo(NPC.Center);
                NPC.velocity = targetPosition * 2;

                head.rot = Utils.AngleLerp(head.rot, targetPosition.ToRotation() + MathHelper.PiOver2, 1 / 20f);
                body.rot = Utils.AngleLerp(body.rot, targetPosition.ToRotation() + MathHelper.PiOver2, 1 / 20f);

                leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, body.rot + MathHelper.ToRadians(24), 1 / 40f);
                leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, body.rot + MathHelper.ToRadians(20), 1 / 40f);

                rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, body.rot + MathHelper.ToRadians(-24), 1 / 40f);
                rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, body.rot + MathHelper.ToRadians(-20), 1 / 40f);
                break;
            case GrabPrepTime:
                PlayRoar();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    grabProjIdentity = LemonUtils.QuickProj(NPC, NPC.Center + grabOffset, Vector2.Zero, ProjectileType<DeathbirdGrab>(), 1, ai0: NPC.whoAmI).identity;
                }
                NPC.netUpdate = true;
                break;
            case > 300:
                Projectile grabProj = Main.projectile.FirstOrDefault(p => p.identity == grabProjIdentity);
                grabProj.Center = NPC.Center + grabOffset.RotatedBy(MathHelper.PiOver2);
                if (AttackTimer > GrabEndTime)
                {
                    head.rot = Utils.AngleLerp(head.rot, targetPosition.ToRotation() + MathHelper.PiOver2, 1 / 20f);
                    body.rot = Utils.AngleLerp(body.rot, targetPosition.ToRotation() + MathHelper.PiOver2, 1 / 20f);

                    leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, -body.rot, 1 / 20f);
                    leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, body.rot + MathHelper.ToRadians(-60), 1 / 20f);

                    rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, body.rot, 1 / 20f);
                    rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, body.rot + MathHelper.ToRadians(60), 1 / 20f);
                    NPC.velocity = -targetPosition * 25; // target position = playerToNPC
                }
                else
                {
                    NPC.velocity *= 0.9f;
                    head.rot = Utils.AngleLerp(head.rot, MathHelper.ToRadians(NPC.velocity.X * 6), 1 / 20f);
                    body.rot = Utils.AngleLerp(body.rot, MathHelper.ToRadians(NPC.velocity.X * 3), 1 / 20f);

                    leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, MathHelper.ToRadians(NPC.velocity.X * 10), 1 / 20f);
                    leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, body.rot + MathHelper.ToRadians(-60), 1 / 20f);

                    rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, MathHelper.ToRadians(NPC.velocity.X * 10), 1 / 20f);
                    rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, body.rot + MathHelper.ToRadians(60), 1 / 20f);

                    if (AITimer % 15 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < LemonUtils.GetDifficulty(); i++)
                            {
                                Vector2 velocity = Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(1, 2);
                                LemonUtils.QuickProj(NPC, NPC.RandomPos(64, 64), velocity, ProjectileType<DeathflameBall>(), projDamage, ai0: 20, ai1: NPC.target);
                            }
                        }
                    }
                }
                break;
            case 300:
                SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.7f, 1f) }, NPC.Center);
                SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.4f, 0.9f) }, NPC.Center);
                break;
            case > 0:
                BasicMovementAnimation();
                if (AITimer % 30 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 2 * LemonUtils.GetDifficulty(); i++)
                        {
                            Vector2 velocity = Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(8, 10);
                            LemonUtils.QuickProj(NPC, NPC.RandomPos(64, 64), velocity, ProjectileType<DeathflameBall>(), projDamage, ai0: 9999, ai1: NPC.target);
                        }
                    }
                }
                break;
            case 0:
                AttackTimer = GrabDuration;
                return;
        }
        AttackTimer--;
    }

    const int FeatherRainDuration = 600;
    const int FeatherRainWindUpTime = 480;
    const int FeatherRainUpTime = 360;
    const int FeatherRainRainTime = 60;
    void FeatherRain()
    {
        BasicMovementAnimation();

        switch (AttackTimer)
        {
            case FeatherRainDuration:
                NPC.velocity = Vector2.Zero;
                if (Main.netMode != NetmodeID.MultiplayerClient && (Main.masterMode || Main.getGoodWorld))
                {
                    LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ProjectileType<SuckyProjectile>(), projDamage, ai0: 1500, ai1: 500, ai2: 15);
                }
                frameDuration = 12;
                break;
            case FeatherRainWindUpTime:
                PlayRoar();
                break;
            case > FeatherRainUpTime:
                if (AttackTimer % 5 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        LemonUtils.QuickProj(NPC, NPC.RandomPos(64, 32), -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15, 15))) * Main.rand.NextFloat(50, 60), ProjectileType<DeathbirdFeatherFX>());
                    }
                }

                if (AttackTimer % 60 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -3; i <= 3; i++)
                        {
                            Vector2 direction = NPC.DirectionTo(player.Center).RotatedBy(i * (MathHelper.Pi / 12));
                            LemonUtils.QuickProj(NPC, NPC.Center, direction * 12, ProjectileType<DeathbirdFeatherSharp>(), projDamage, ai0: NPC.target, ai1: 1, ai2: 9999);
                        }
                    }
                }
                NPC.velocity.X = (float)Math.Sin(AttackTimer / 30f) * 5;
                NPC.velocity += NPC.DirectionTo(player.Center - Vector2.UnitY * 200) * 0.2f;
                break;
            case > FeatherRainRainTime:
                NPC.velocity *= 0.9f;
                frameDuration = 6;
                if (AttackTimer % 15 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        LemonUtils.QuickProj(NPC, player.Center + new Vector2(Main.rand.Next(-500, 500), -800), Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15, 15))) * Main.rand.NextFloat(25, 35), ProjectileType<DeathbirdFeather>(), projDamage, ai0: 75 - (LemonUtils.GetDifficulty() * 5), ai1: 3);
                    }
                }
                break;
            case 0:
                AttackTimer = FeatherRainDuration;
                return;
        }
        AttackTimer--;
    }

    const int LingeringFlameRainDuration = 180;
    const int LingeringFlameRainMoveToPosTime = 120;
    const int LingeringFlameRainMoveToPosTime2 = 30;
    void LingeringFlameRain()
    {
        frameDuration = 6;
        BasicMovementAnimation();
        int direction = AttackCount % 2 == 0 ? -1 : 1;
        switch (AttackTimer)
        {
            case LingeringFlameRainDuration:
                targetPosition = player.Center + new Vector2(direction * 600, -200);
                break;
            case > LingeringFlameRainMoveToPosTime:
                NPC.MoveToPos(targetPosition, 0.3f, 0.2f, 0.5f, 0.3f);
                break;
            case LingeringFlameRainMoveToPosTime:
                targetPosition = NPC.Center + -direction * 1000 * Vector2.UnitX;
                AttackCount++;
                break;
            case > LingeringFlameRainMoveToPosTime2:
                if (attackDuration < 120)
                {
                    DrawClone(player.Center - Vector2.UnitY * 300, 0.008f, 2f);
                }
                NPC.MoveToPos(targetPosition, 0.3f, 0.1f, 0.5f, 0.1f);
                if (AttackTimer % 10 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int count = NPC.CountNPCS(NPCType<GiantUndead>()) > 0 ? 1 : 3;
                        for (int i = 0; i < count; i++)
                        {
                            LemonUtils.QuickProj(NPC, NPC.RandomPos(64, 32), Vector2.UnitY * 3, ProjectileType<LingeringDeathflame>(), projDamage, ai0: NPC.target, ai1: 600);
                        }
                    }
                }
                break;
            case > 0:
                if (attackDuration < 120)
                {
                    DrawClone(player.Center - Vector2.UnitY * 300, 0.008f, 2f);
                }
                NPC.velocity *= 0.9f;
                break;
            case 0:
                AttackTimer = LingeringFlameRainDuration;
                return;
        }
        AttackTimer--;
    }

    const int ArenaDeathflameDuration = 1200;
    int arenaProgressTimer = 0;
    int baseArenaDistance = 1000;
    int arenaDistance = 1000;
    void ArenaDeathflame()
    {
        if (AttackTimer % 10 == 0)
        {
            foreach (var p in Main.ActivePlayers)
            {
                if (p.Distance(NPC.Center) > arenaDistance - 200)
                {
                    p.AddBuff(BuffType<DeathflameDebuff>(), 15);
                }
            }
        }
        ScreenShaderData data = Terraria.Graphics.Effects.Filters.Scene.Activate("NeoParacosm:DeathbirdArenaShader").GetShader();
        data.UseImage(ParacosmTextures.NoiseTexture, 1);
        data.UseTargetPosition(NPC.Center);
        data.Shader.Parameters["targetPosition2"].SetValue(NPC.Center + Vector2.UnitX * arenaDistance);
        data.Shader.Parameters["time"].SetValue(AITimer / 60f);
        data.UseProgress(arenaProgressTimer / 60f);
        BasicMovementAnimation();
        frameDuration = 6;
        switch (AttackTimer)
        {
            case ArenaDeathflameDuration:
                TeleportToPos(player.Center - Vector2.UnitY * 300);
                break;
            case > ArenaDeathflameDuration - 60:
                NPC.velocity = Vector2.Zero;
                arenaProgressTimer++;
                break;
            case > 60:
                if (AttackTimer % 120 == 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, Vector2.UnitY.RotatedBy(i * MathHelper.PiOver4) * 7, ProjectileType<DeathbirdFeatherSharp>(), projDamage, ai0: NPC.target, ai1: 60, ai2: 60);
                    }
                    AttackCount++;
                }
                break;
            case > 0:
                arenaProgressTimer--;
                arenaDistance += 5;
                break;
            case 0:
                AttackTimer = ArenaDeathflameDuration;
                arenaDistance = baseArenaDistance;
                return;
        }
        AttackTimer--;
    }

    const int DeathflameKamikazeDuration = 180;
    void DeathflameKamikaze()
    {
        BasicMovementAnimation();
        frameDuration = 6;
        switch (AttackTimer)
        {
            case DeathflameKamikazeDuration:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    targetPosition = player.Center + Main.rand.NextVector2Circular(300, 300);
                }
                AttackCount = 0;
                NPC.netUpdate = true;
                break;
            case > 60:
                AttackCount++;
                float angle = MathHelper.ToRadians(45 - AttackCount * 3);
                float distance = 800 - AttackCount * 20;
                if (distance < 0) distance = 0;
                Vector2 dustOffset = Vector2.UnitY.RotatedBy(angle) * distance;
                if (AttackTimer % 2 == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 dustPos = targetPosition + dustOffset.RotatedBy(MathHelper.PiOver2 * i);
                        Dust.NewDustPerfect(dustPos, DustID.GemDiamond, Vector2.Zero, newColor: Color.Black, Scale: 3f).noGravity = true;
                    }
                }
                NPC.MoveToPos(targetPosition, 0.6f, 0.6f, 0.4f, 0.4f);
                break;
            case 60:
                PlayRoar();
                if (Main.netMode != NetmodeID.MultiplayerClient && (Main.masterMode || Main.getGoodWorld))
                {
                    LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ProjectileType<SuckyProjectile>(), projDamage, ai0: 1500, ai1: 60, ai2: 0);
                }
                NPC.velocity = Vector2.Zero;
                for (int i = 0; i < 5; i++)
                {
                    int dustID = i % 2 != 0 ? DustID.GemDiamond : DustID.Ash;
                    Color dustColor = i % 2 != 0 ? Color.White : Color.Black;
                    LemonUtils.DustCircle(NPC.Center, 8, (i + 1) * 6, dustID, (i + 2), color: dustColor);
                }

                int projAmount = LemonUtils.GetDifficulty() * 12;
                for (int i = 0; i < projAmount; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float randomAngle = MathHelper.ToRadians(Main.rand.Next(-15, 15));
                        Vector2 velocity = Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / projAmount)).RotatedBy(randomAngle) * Main.rand.NextFloat(10, 17);
                        LemonUtils.QuickProj(NPC, NPC.Center, velocity, ProjectileType<DeathflameBall>(), projDamage, ai0: 999, ai1: NPC.target);
                        LemonUtils.QuickProj(NPC, NPC.Center, Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(6, 10), ProjectileType<LingeringDeathflame>(), projDamage, ai0: -1, ai1: 300);
                    }

                }
                SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                SoundEngine.PlaySound(SoundID.Zombie93, NPC.Center);
                SoundEngine.PlaySound(SoundID.Zombie103, NPC.Center);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                break;
            case 0:
                AttackTimer = DeathflameKamikazeDuration;
                return;
        }
        AttackTimer--;
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

    void Intro()
    {
        NPC.dontTakeDamage = true;
        NPC.velocity = Vector2.Zero;
        if (AITimer == 0)
        {
            SetDefaultBodyPartValues();
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (0.2f, 0.5f) }, NPC.Center);
        }

        if (AITimer < IntroDuration - 120)
        {
            NPC.Opacity = 0f;
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustDirect(NPC.RandomPos(16, 16), 2, 2, DustID.Ash, Scale: Main.rand.NextFloat(1f, 4f), newColor: Color.Black).noGravity = true;
                Dust.NewDustDirect(NPC.RandomPos(16, 16), 2, 2, DustID.GemDiamond, Scale: Main.rand.NextFloat(1f, 4f), newColor: Color.White).noGravity = true;
            }
        }
        else if (AITimer == IntroDuration - 120)
        {
            NPC.Opacity = 1f;
            for (int i = 0; i < 3; i++)
            {
                LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(2f, 4f));
            }
            PunchCameraModifier mod1 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 90, 1000f, FullName);
            Main.instance.CameraModifiers.Add(mod1);
            PlayRoar();
        }
        attackDuration = attackDurations[(int)Attack];
    }

    const int PhaseTransitionDuration = 240;
    float maxRainingValue = 0.7f;
    void PhaseTransition()
    {
        NPC.dontTakeDamage = true;
        SetDefaultBodyPartValues();
        NPC.velocity = Vector2.Zero;
        frameDuration = 6;
        BasicMovementAnimation();
        drawClone = false;
        switch (AttackTimer)
        {
            case < 120:
                if (AttackTimer == 0)
                {
                    Main.StartRain();
                }
                Main.maxRaining = MathHelper.Lerp(Main.maxRaining, maxRainingValue, AttackTimer / 120f);
                Main.cloudAlpha = MathHelper.Lerp(Main.cloudAlpha, maxRainingValue, AttackTimer / 120f);
                if (AttackTimer % 10 == 0)
                {
                    LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(1f, 4f));
                }
                PunchCameraModifier mod1 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 10f, 6f, 10, 1000f, FullName);
                Main.instance.CameraModifiers.Add(mod1);
                break;
            case 120:
                wingOutlineScale = 1.35f;
                wingScale = 1.1f;
                darkColorBoost = 1f;
                LemonUtils.DustCircle(NPC.Center, 24, 24, DustID.GemDiamond, 8f);
                PunchCameraModifier mod2 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 30, 1000f, FullName);
                Main.instance.CameraModifiers.Add(mod2);
                PlayRoar();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ProjectileType<PulseEffect>(), ai0: 1, ai1: 20, ai2: 2);
                }
                break;
            case < PhaseTransitionDuration:
                if (AITimer % 20 == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 pos = NPC.Center - (Vector2.UnitY * 300).RotatedBy(i * MathHelper.ToRadians(120));
                        LemonUtils.DustCircle(pos, 8, 8, DustID.Ash, 4f);
                    }
                }

                if (AttackTimer <= PhaseTransitionDuration - 150 && AttackTimer % 10 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ProjectileType<PulseEffect>(), ai0: 1, ai1: 20, ai2: 2);
                    }
                }
                break;
            case PhaseTransitionDuration:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 pos = NPC.Center - (Vector2.UnitY * 300).RotatedBy(i * MathHelper.ToRadians(120));
                        NPC.NewNPCDirect(NPC.GetSource_FromAI(), pos, NPCType<GiantUndead>(), ai3: NPC.whoAmI);
                    }
                }
                phase = 2;
                phaseTransition = false;
                Attack = 0;
                AttackTimer = 0;
                attackDuration = attackDurations2[(int)Attack];
                return;
        }
        AttackTimer++;
    }

    public override void OnKill()
    {

    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ) return;
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 3; i++)
            {
                LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(2f, 4f));
            }
            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), GoreType<DeathbirdGore>());
            }
            Gore.NewGore(NPC.GetSource_FromThis(), head.pos, Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), GoreType<DeathbirdHeadGore>());
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        LeadingConditionRule classicRule = new LeadingConditionRule(new Conditions.NotExpert());
        classicRule.OnSuccess(ItemDropRule.OneFromOptions(1, ItemType<Gravesword>(), ItemType<DeathTolls>(), ItemType<LamentOfTheLate>(), ItemType<StarecrowStaff>()));
        npcLoot.Add(classicRule);
        npcLoot.Add(ItemDropRule.BossBag(ItemType<DeathbirdTreasureBag>()));
        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<DeathbirdRelicItem>()));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    void TeleportToPos(Vector2 pos)
    {
        LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.Ash, Main.rand.NextFloat(3f, 4f), color: Color.Black);
        NPC.Center = pos;
        LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(3f, 4f));
        SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned with { PitchRange = (0f, 0.2f) }, NPC.Center);
    }

    void SetBodyPartPositions()
    {
        body.pos = NPC.Center;
        head.pos = body.pos - Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.5f;
        leftLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.4f;
        leftLeg2.pos = leftLeg1.pos + new Vector2(-leftLeg1Texture.Width() * 0.5f, leftLeg1Texture.Height() * 0.8f).RotatedBy(leftLeg1.rot);
        rightLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.4f;
        rightLeg2.pos = rightLeg1.pos + new Vector2(rightLeg1Texture.Width() * 0.5f, rightLeg1Texture.Height() * 0.8f).RotatedBy(rightLeg1.rot);
    }

    void PlayRoar()
    {
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0f, 1f) }, NPC.Center);
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.2f, 0.4f) }, NPC.Center);
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.5f, 0.7f) }, NPC.Center);
    }

    Color defaultColor => Color.White * NPC.Opacity;
    bool drawClone = false;
    Vector2 clonePos = Vector2.Zero;
    float cloneOpacity = 0f;
    float cloneScale = 1f;
    void DrawClone(Vector2 pos, float opacityIncrementValue, float scale)
    {
        drawClone = true;
        clonePos = pos;
        cloneOpacity += opacityIncrementValue;
        cloneScale = scale;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.Opacity < 1)
        {
            return false;
        }

        // Setting up positions and origin

        //NPC.rotation += MathHelper.ToRadians(1);
        NPC.rotation = 0;

        // Wings
        var shader = GameShaders.Misc["NeoParacosm:DeathbirdWingShader"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["tolerance"].SetValue(0.5f);
        shader.Shader.Parameters["darkColorBoost"].SetValue(darkColorBoost);
        shader.Shader.Parameters["color"].SetValue(Color.White.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.75f);

        // First the "outline"/afterimage/effect wings
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Vector2 wingLeftPos = body.pos;
        Vector2 wingLeftOrigin = new Vector2(wingsLeftTexture.Frame(1, 5, 0, 0).Width, wingsLeftTexture.Frame(1, 5, 0, 0).Height / 2);
        Vector2 wingRightPos = body.pos;
        Vector2 wingRightOrigin = new Vector2(0, wingsRightTexture.Frame(1, 5, 0, 0).Height / 2);
        Main.EntitySpriteDraw(wingsLeftTexture.Value, wingLeftPos - Main.screenPosition, wingsLeftTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingLeftOrigin, NPC.scale * wingOutlineScale, SpriteEffects.None, 0);
        shader.Shader.Parameters["moveSpeed"].SetValue(-0.75f);
        Main.EntitySpriteDraw(wingsRightTexture.Value, wingRightPos - Main.screenPosition, wingsRightTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingRightOrigin, NPC.scale * wingOutlineScale, SpriteEffects.None, 0);
        Main.spriteBatch.End();

        // Actual wings
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Shader.Parameters["moveSpeed"].SetValue(0.75f);
        Main.EntitySpriteDraw(wingsLeftTexture.Value, wingLeftPos - Main.screenPosition, wingsLeftTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingLeftOrigin, NPC.scale * wingScale, SpriteEffects.None, 0);
        shader.Shader.Parameters["moveSpeed"].SetValue(-0.75f);
        Main.EntitySpriteDraw(wingsRightTexture.Value, wingRightPos - Main.screenPosition, wingsRightTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingRightOrigin, NPC.scale * wingScale, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


        // Body
        Main.EntitySpriteDraw(bodyTexture.Value, body.pos - Main.screenPosition, null, defaultColor, body.rot, body.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(headTexture.Value, head.pos - Main.screenPosition, null, defaultColor, head.rot, head.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(leftLeg1Texture.Value, leftLeg1.pos - Main.screenPosition, null, defaultColor, leftLeg1.rot, leftLeg1.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(leftLeg2Texture.Value, leftLeg2.pos - Main.screenPosition, null, defaultColor, leftLeg2.rot, leftLeg2.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(rightLeg1Texture.Value, rightLeg1.pos - Main.screenPosition, null, defaultColor, rightLeg1.rot, rightLeg1.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(rightLeg2Texture.Value, rightLeg2.pos - Main.screenPosition, null, defaultColor, rightLeg2.rot, rightLeg2.origin, NPC.scale, SpriteEffects.None);

        // Clone
        if (drawClone && clonePos != Vector2.Zero)
        {
            Main.EntitySpriteDraw(headTexture.Value, clonePos - Main.screenPosition, null, Color.White * cloneOpacity, 0f, head.origin, cloneScale, SpriteEffects.None);
        }
        return false;
    }
}
