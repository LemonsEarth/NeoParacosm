using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Content.Projectiles.Hostile;
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
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;
using Terraria.UI;

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

    float attackDuration = 0;
    int[] attackDurations = { 600, 600, 600, 600, 600 };

    public enum Attacks
    {
        DiagonalSwings,
    }

    int wingFrame = 0;
    int wingAnimTimer = 0;
    #endregion

    #region Constants
    const int INTRO_DURATION = 60;

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

    #endregion

    int facingDirection = 1;

    Vector2 lookPosition = Vector2.Zero;
    public Player player { get; private set; }


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
                SetBodyPartPositions(1 / 5f, 1 / 5f, 1 / 10f);
                if (NPC.Center.Y < player.Center.Y)
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
                MoveCameraModifier cameraModifier = new MoveCameraModifier(NPC.Center, () => AITimer > 480 || !NPC.active);
                Main.instance.CameraModifiers.Add(cameraModifier);
                break;
            case < 180:
                SetBodyPartPositions(HeadCorrupt.DefaultPosition + new Vector2(0, 50),
                                     HeadCrimson.DefaultPosition + new Vector2(0, 50),
                                     LegCorrupt.DefaultPosition,
                                     LegCrimson.DefaultPosition,
                                     Body.DefaultPosition + new Vector2(0, 20),
                                     1 / 10f,
                                     1 / 10f,
                                     1 / 10f
                                     );
                break;
            case 210:
                NPC.velocity = Vector2.Zero;
                NPC.noTileCollide = true;
                SoundEngine.PlaySound(SoundID.Roar with { MaxInstances = 2, Pitch = -1f }, NPC.Center);
                SoundEngine.PlaySound(SoundID.NPCDeath62 with { MaxInstances = 2, Pitch = -0.5f }, NPC.Center);
                PunchCameraModifier mod1 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 60f, 6f, 360, 1000f, FullName);
                Main.instance.CameraModifiers.Add(mod1);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectileDirect(
                    NPC.GetSource_FromThis(),
                    Vector2.Lerp(HeadCorrupt.Position, HeadCrimson.Position, 0.5f),
                    Vector2.Zero,
                    ProjectileType<PulseEffect>(),
                    0,
                    0,
                    -1,
                    4, 15, 5
                    );
                }
                break;
            case < 480 and > 210:
                if (AITimer % 15 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectileDirect(
                        NPC.GetSource_FromThis(),
                        Vector2.Lerp(HeadCorrupt.Position, HeadCrimson.Position, 0.5f),
                        Vector2.Zero,
                        ProjectileType<PulseEffect>(),
                        0,
                        0,
                        -1,
                        2, 15, 5
                        );
                    }
                }
                SetBodyPartPositions(HeadCorrupt.DefaultPosition - new Vector2(0, 50),
                                    HeadCrimson.DefaultPosition - new Vector2(0, 50),
                                    LegCorrupt.DefaultPosition,
                                    LegCrimson.DefaultPosition,
                                    Body.DefaultPosition
                                    );
                HeadCorrupt.CurrentFrame = HEAD_MOUTH_OPEN;
                HeadCrimson.CurrentFrame = HEAD_MOUTH_OPEN;
                break;
            case < 540 and > 210:
                SetBodyPartPositions();
                HeadCorrupt.CurrentFrame = HEAD_MOUTH_CLOSED;
                HeadCrimson.CurrentFrame = HEAD_MOUTH_CLOSED;
                break;
        }

        //NPC.velocity = Vector2.Zero;
        attackDuration = attackDurations[(int)Attack];
    }

    void AttackControl()
    {
        switch (Attack)
        {
            case (int)Attacks.DiagonalSwings:
                DiagonalSwings();
                break;
        }

        attackDuration--;
        if (attackDuration <= 0)
        {
            SwitchAttacks();
        }
    }

    void DiagonalSwings()
    {
        SetBodyPartPositions(1f, 1f, 1f);
        switch (AttackTimer)
        {
            case 300:
                AttackCount++;
                break;
            case > 180:
                Vector2 diagPos = player.Center + new Vector2(-500, -700);
                NPC.MoveToPos(diagPos, 0.2f, 0.2f, 0.2f, 0.2f);
                break;
            case > 150:
                NPC.velocity = player.DirectionTo(NPC.Center) * 5;
                LegCorrupt.CurrentFrame = LEG_ATTACK;
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, MathHelper.PiOver2, 1 / 40f);
                break;
            case 150:
                NPC.velocity = NPC.DirectionTo(player.Center) * 25;
                break;
            case >= 120:
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, -MathHelper.Pi / 6, 1 / 15f);
                break;
            case > 30 and < 120:
                LegCorrupt.Rotation = Utils.AngleLerp(LegCorrupt.Rotation, 0, 1 / 40f);
                NPC.velocity *= 0.97f;
                break;
            case 0:
                NPC.velocity = Vector2.Zero;
                LegCorrupt.CurrentFrame = LEG_STANDARD;
                AttackTimer = 300;
                return;
        }

        AttackTimer--;
    }

    void SwitchAttacks()
    {
        Attack++;
        attackDuration = attackDurations[(int)Attack];

        AttackCount = 0;
        AttackTimer = 0;
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


    public override bool CheckDead()
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

        LegCorrupt.DefaultPosition = Body.Position + new Vector2(-Body.Width * 0.57f, -Body.Height * 0.35f);
        LegCorrupt.Origin = new Vector2(LegCorrupt.Width * 0.5f, 0);
        LegCorrupt.Frames = 2;

        LegCrimson.DefaultPosition = Body.Position + new Vector2(Body.Width * 0.57f, -Body.Height * 0.35f);
        LegCrimson.Origin = new Vector2(LegCrimson.Width * 0.5f, 0);
        LegCrimson.Frames = 2;

        BackLegs.DefaultPosition = Body.Position + new Vector2(0, BackLegs.Height * 0.2f);
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

    public void SetBodyPartPositions(float headLerpSpeed = 1 / 10f, float legLerpSpeed = 1 / 10f, float bodyLerpSpeed = 1 / 10f)
    {
        Body.Position = Vector2.Lerp(Body.Position, Body.DefaultPosition, bodyLerpSpeed);

        LegCorrupt.Position = Vector2.Lerp(LegCorrupt.Position, LegCorrupt.DefaultPosition, legLerpSpeed);
        LegCrimson.Position = Vector2.Lerp(LegCrimson.Position, LegCrimson.DefaultPosition, legLerpSpeed);

        BackLegs.Position = BackLegs.DefaultPosition;

        HeadCorrupt.Position = Vector2.Lerp(HeadCorrupt.Position, HeadCorrupt.DefaultPosition, headLerpSpeed);
        HeadCrimson.Position = Vector2.Lerp(HeadCrimson.Position, HeadCrimson.DefaultPosition, headLerpSpeed);

        WingCorrupt.Position = WingCorrupt.DefaultPosition;
        WingCrimson.Position = WingCrimson.DefaultPosition;

    }

    public void SetBodyPartPositions(Vector2 headCorrupttargetPosition,
                                    Vector2 headCrimsonTargetPosition,
                                    Vector2 legCorruptTargetPosition,
                                    Vector2 legCrimsonTargetPosition,
                                    Vector2 bodyTargetPosition,
                                    float headLerpSpeed = 1 / 10f,
                                    float legLerpSpeed = 1 / 10f,
                                    float bodyLerpSpeed = 1 / 10f)
    {
        Body.Position = Vector2.Lerp(Body.Position, bodyTargetPosition, bodyLerpSpeed);

        LegCorrupt.Position = Vector2.Lerp(LegCorrupt.Position, legCorruptTargetPosition, legLerpSpeed);
        LegCrimson.Position = Vector2.Lerp(LegCrimson.Position, legCrimsonTargetPosition, legLerpSpeed);

        BackLegs.Position = BackLegs.DefaultPosition;
        HeadCorrupt.Position = Vector2.Lerp(HeadCorrupt.Position, headCorrupttargetPosition, headLerpSpeed);
        HeadCrimson.Position = Vector2.Lerp(HeadCrimson.Position, headCrimsonTargetPosition, headLerpSpeed);

        WingCorrupt.Position = WingCorrupt.DefaultPosition;
        WingCrimson.Position = WingCrimson.DefaultPosition;

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
            drawPos += baseToDestination * texture.Height() * 0.9f;
            distanceLeft -= texture.Height() * 0.9f;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        BackLegs.Draw();
        WingCorrupt.Draw();
        WingCrimson.Draw();
        Body.Draw();
        LegCorrupt.Draw();
        LegCrimson.Draw();
        DrawNeck(Body.MiscPosition1, HeadCorrupt.Position, neckTextureCorrupt);
        DrawNeck(Body.MiscPosition2, HeadCrimson.Position, neckTextureCrimson);
        HeadCorrupt.Draw();
        HeadCrimson.Draw();
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

    }
}
