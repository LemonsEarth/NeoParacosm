using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Projectiles.Hostile;
using NeoParacosm.Core.Systems;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
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
            int maxVal = phase == 1 ? 2 : 3;
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
    int[] attackDurations = { 900, 900, 720, 1200, 600 };
    int[] attackDurations2 = { 900, 900, 720, 720, 900, 1080, 960 };
    public Player player { get; private set; }
    Vector2 targetPosition = Vector2.Zero;

    float projDamage => NPC.damage / 2;

    public enum Attacks
    {
        HomingDeathflameBalls,
        HoverLingeringFlame,
        LaserFeathers,
    }

    public enum Attacks2
    {

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
        NPC.lifeMax = 12000;
        NPC.defense = 13;
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
            //Music = MusicLoader.GetMusicSlot(Mod, "Content/Audio/Music/AnotherSamePlace");
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

    int INTRO_DURATION = 300;
    int TRANSITION_DURATION = 300;
    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];

        body.pos = NPC.Center;
        head.pos = body.pos - Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.5f;
        leftLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.4f;
        leftLeg2.pos = leftLeg1.pos + new Vector2(-leftLeg1Texture.Width() * 0.5f, leftLeg1Texture.Height() * 0.8f).RotatedBy(leftLeg1.rot);
        rightLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.4f;
        rightLeg2.pos = rightLeg1.pos + new Vector2(rightLeg1Texture.Width() * 0.5f, rightLeg1Texture.Height() * 0.8f).RotatedBy(rightLeg1.rot);

        DespawnCheck();
        if (AITimer < INTRO_DURATION)
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
            }
        }
        else if (phase == 2)
        {

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
        head.rot = Utils.AngleLerp(head.rot, MathHelper.ToRadians(NPC.velocity.X * 6), 1 / 20f);
        body.rot = Utils.AngleLerp(body.rot, MathHelper.ToRadians(NPC.velocity.X * 3), 1 / 20f);

        leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, MathHelper.ToRadians(NPC.velocity.X * 10), 1 / 20f);
        leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, MathHelper.ToRadians(NPC.velocity.X * 10), 1 / 20f);

        rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, MathHelper.ToRadians(NPC.velocity.X * 10), 1 / 20f);
        rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, MathHelper.ToRadians(NPC.velocity.X * 10), 1 / 20f);
    }

    void SwitchAttacks()
    {
        PlayRoar();
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            Attack++;
            if (phase == 1) attackDuration = attackDurations[(int)Attack];
            else attackDuration = attackDurations2[(int)Attack];

            AttackCount = 0;
            AttackTimer = 0;
            NPC.Opacity = 1f;
        }
        NPC.netUpdate = true;
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
                int minYValue = AttackCount % 2 == 0 ? -500 : -250;
                int maxYValue = AttackCount % 2 == 0 ? 250 : 500;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    targetPosition = NPC.Center + new Vector2(Main.rand.Next(-250, 250), Main.rand.Next(minYValue, maxYValue));
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
                NPC.velocity *= 0.99f;
                if (AttackTimer % 60 == 0)
                {
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
                frameDuration = 999;
                break;
            case > 0:
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
                LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.Ash, Main.rand.NextFloat(3f, 4f), color: Color.Black);
                NPC.Center = player.Center - Vector2.UnitY * 300;
                LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(3f, 4f));
                SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned with { PitchRange = (0f, 0.2f) }, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = -8; i <= 8; i++)
                    {
                        Vector2 pos = NPC.Center + Vector2.UnitX * i * 100;
                        LemonUtils.QuickProj(NPC, pos, new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(2, 4)), ProjectileType<LingeringDeathflame>(), projDamage, ai0: player.whoAmI);
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
                            LemonUtils.QuickProj(NPC, NPC.RandomPos(), Vector2.UnitY * 3, ProjectileType<LingeringDeathflame>(), projDamage);
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

        if (AITimer < INTRO_DURATION - 120)
        {
            NPC.Opacity = 0f;
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustDirect(NPC.RandomPos(16, 16), 2, 2, DustID.Ash, Scale: Main.rand.NextFloat(1f, 4f), newColor: Color.Black).noGravity = true;
                Dust.NewDustDirect(NPC.RandomPos(16, 16), 2, 2, DustID.GemDiamond, Scale: Main.rand.NextFloat(1f, 4f), newColor: Color.White).noGravity = true;
            }
        }
        else if (AITimer == INTRO_DURATION - 120)
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

    float wingScale = 1.05f;
    float darkColorBoost = 0f;
    void PhaseTransition()
    {
        NPC.dontTakeDamage = true;
        SetDefaultBodyPartValues();
        NPC.velocity = Vector2.Zero;
        switch (AttackTimer)
        {
            case < 120:
                if (AttackTimer % 10 == 0)
                {
                    LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(1f, 4f));
                }
                break;
            case 120:
                wingScale = 1.25f;
                darkColorBoost = 1f;
                LemonUtils.DustCircle(NPC.Center, 24, 24, DustID.GemDiamond, 8f);
                PunchCameraModifier mod1 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 90, 1000f, FullName);
                Main.instance.CameraModifiers.Add(mod1);
                PlayRoar();
                break;
            case 300:
                phase = 2;
                phaseTransition = false;
                return;
        }
        AttackTimer++;
    }

    public override void OnKill()
    {

    }

    public override void HitEffect(NPC.HitInfo hit)
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    void PlayRoar()
    {
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0f, 1f) }, NPC.Center);
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.2f, 0.4f) }, NPC.Center);
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.5f, 0.7f) }, NPC.Center);
    }

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

    Color defaultColor => Color.White * NPC.Opacity;
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
        Main.EntitySpriteDraw(wingsLeftTexture.Value, wingLeftPos - Main.screenPosition, wingsLeftTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingLeftOrigin, NPC.scale * wingScale, SpriteEffects.None, 0);
        shader.Shader.Parameters["moveSpeed"].SetValue(-0.75f);
        Main.EntitySpriteDraw(wingsRightTexture.Value, wingRightPos - Main.screenPosition, wingsRightTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingRightOrigin, NPC.scale * wingScale, SpriteEffects.None, 0);
        Main.spriteBatch.End();

        // Actual wings
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Shader.Parameters["moveSpeed"].SetValue(0.75f);
        Main.EntitySpriteDraw(wingsLeftTexture.Value, wingLeftPos - Main.screenPosition, wingsLeftTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingLeftOrigin, NPC.scale, SpriteEffects.None, 0);
        shader.Shader.Parameters["moveSpeed"].SetValue(-0.75f);
        Main.EntitySpriteDraw(wingsRightTexture.Value, wingRightPos - Main.screenPosition, wingsRightTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingRightOrigin, NPC.scale, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


        // Body
        Main.EntitySpriteDraw(bodyTexture.Value, body.pos - Main.screenPosition, null, defaultColor, body.rot, body.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(headTexture.Value, head.pos - Main.screenPosition, null, defaultColor, head.rot, head.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(leftLeg1Texture.Value, leftLeg1.pos - Main.screenPosition, null, defaultColor, leftLeg1.rot, leftLeg1.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(leftLeg2Texture.Value, leftLeg2.pos - Main.screenPosition, null, defaultColor, leftLeg2.rot, leftLeg2.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(rightLeg1Texture.Value, rightLeg1.pos - Main.screenPosition, null, defaultColor, rightLeg1.rot, rightLeg1.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(rightLeg2Texture.Value, rightLeg2.pos - Main.screenPosition, null, defaultColor, rightLeg2.rot, rightLeg2.origin, NPC.scale, SpriteEffects.None);
        return false;
    }
}
