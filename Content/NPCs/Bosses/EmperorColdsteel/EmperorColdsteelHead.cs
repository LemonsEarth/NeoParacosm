using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Hostile.Ice.EmperorColdsteel;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using static Microsoft.Xna.Framework.MathHelper;

namespace NeoParacosm.Content.NPCs.Bosses.EmperorColdsteel;

[AutoloadBossHead]
public class EmperorColdsteelHead : ModNPC
{
    int BodyType => NPCType<EmperorColdsteelBody>();
    const int MAX_SEGMENT_COUNT = 8;
    public int SegmentCount { get; private set; } = 0;
    bool debugText = false;
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
    readonly int[] attackDurations = [1080, 900, 1080, 1080, 1320];
    readonly int[] attackDurations2 = [1600, 1080, 1080, 1080, 1320];

    /// <summary>
    /// Attacks that can be performed (order matters)
    /// </summary>
    public enum Attacks
    {
        DivingOnPlayer,
        SineIceBurst
    }

    public enum Attacks2
    {
        Pillars
    }

    public int Phase { get; private set; } = 0;
    public int MiniPhase { get; private set; } = 0;
    #endregion

    public Player player { get; private set; }
    Vector2 targetPosition = Vector2.Zero;

    List<EmperorColdsteelBody> Segments = new List<EmperorColdsteelBody>();

    bool drawFlashlight = false;
    Color flashlightColor = Color.White;
    float flashlightOpacity = 0f;

    public override void SetStaticDefaults()
    {
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
        Main.npcFrameCount[NPC.type] = 1;
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
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
            new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Bestiary")),
        });
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {

    }

    public override void SetDefaults()
    {
        NPC.width = 128;
        NPC.height = 128;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.Opacity = 1f;
        NPC.lifeMax = 400000;
        NPC.defense = 40;
        NPC.damage = 100;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 30000;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 10;
        NPC.hide = true;
        NPC.SpawnWithHigherTime(30);

        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Common/Assets/Audio/Music/ChaosCognition");
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

    void Visuals()
    {
        Lighting.AddLight(NPC.Center, 0.8f, 0.8f, 1f);
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        if (drawFlashlight)
        {
            flashlightOpacity = Lerp(flashlightOpacity, 1f, 1 / 30f);
        }
        else
        {
            flashlightOpacity = Lerp(flashlightOpacity, 0f, 1 / 10f);
        }
    }

    void PhaseControl()
    {
        if (NPC.GetLifePercent() < 0.8f && MiniPhase < 1)
        {
            MiniPhase = 1;
        }
    }

    public override void AI()
    {
        debugText = false;
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        if (AITimer == 0)
        {
            SpawnSegments();
            targetPosition = NPC.Center;
            NPC.velocity = Vector2.UnitY * 5;
            attackDuration = attackDurations[0];
        }
        player = Main.player[NPC.target];

        PhaseControl();
        Visuals();
        MoveToPos(player.Center, 30, 10);
        DespawnCheck();

        AttackControl();

        AITimer++;
    }

    void DoDrawFlashlight(Color color)
    {
        drawFlashlight = true;
        flashlightColor = color;
    }

    void StopDrawingFlashlight()
    {
        drawFlashlight = false;
    }

    void MoveToPos(Vector2 pos, float turningSpeedDegreesDenominator = 60f, float moveSpeed = 8f)
    {
        Vector2 dirToPos = NPC.DirectionTo(pos);
        float angleBetween = MathHelper.ToDegrees(LemonUtils.AngleBetween(NPC.velocity, dirToPos));
        //Main.NewText(angleBetween);
        if (MathF.Abs(angleBetween) > MathHelper.ToRadians(5))
        {
            NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(angleBetween / turningSpeedDegreesDenominator)) * moveSpeed;
        }

    }

    void SpawnSegments()
    {
        int latestNPC = NPC.whoAmI;
        while (SegmentCount < MAX_SEGMENT_COUNT - 1) // Body segments, excluding head
        {
            latestNPC = SpawnSegment(BodyType, latestNPC);
            EmperorColdsteelBody bodySegment = (EmperorColdsteelBody)Main.npc[latestNPC].ModNPC;
            Segments.Add(bodySegment);
            SegmentCount++;
        }
    }

    int SpawnSegment(int type, int latestNPC)
    {
        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, 0, latestNPC, NPC.whoAmI, SegmentCount);
        Main.npc[latestNPC].realLife = NPC.whoAmI;
        return latestNPC;
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

    void AttackControl()
    {
        if (Phase == 0)
        {
            switch (Attack)
            {
                case (int)Attacks.DivingOnPlayer:
                    DivingOnPlayer();
                    break;
                case (int)Attacks.SineIceBurst:
                    SineIceBurst();
                    break;
                    /*case (int)Attacks.CursedFlamethrower:
                        break;
                    case (int)Attacks.FlameWallsAndSpinning:
                        break;
                    case (int)Attacks.DashingWithBalls:
                        break; */
            }
        }
        else if (Phase == 1)
        {
            switch (Attack)
            {
                case (int)Attacks2.Pillars:
                    break;
            }
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
        foreach (EmperorColdsteelBody bodySegment in Segments)
        {
            bodySegment.SwitchAttacks((int)Attack);
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
        NPC.Opacity = 1f;
    }

    void DivingOnPlayer()
    {
        switch (AttackTimer)
        {
            case 360: // Repeat this several times
                targetPosition = player.Center - Vector2.UnitY * 1000;
                break;
            case > 240: // Moving above player
                Print("Move above player");
                MoveToPos(player.Center - Vector2.UnitY * 1000, 16, 20);
                if (AttackTimer < 300)
                {
                    DoDrawFlashlight(Color.Red);
                }
                break;
            case 240:
                StopDrawingFlashlight();
                break;
            case > 120: // Moving to player, "circling"
                Print("Move to player");
                MoveToPos(player.Center, 30, 50);
                if (MiniPhase >= 1)
                {
                    if (AttackTimer == 150)
                    {
                        foreach (var bodySegment in Segments)
                        {
                            if (LemonUtils.NotClient())
                            {
                                LemonUtils.QuickProj(
                                    NPC,
                                    bodySegment.NPC.Center,
                                    Vector2.Zero,
                                    ProjectileType<IceBurst>(),
                                    ai0: 45,
                                    ai1: 60,
                                    ai2: 8
                                    );
                            }
                        }
                    }
                    if (AttackTimer % 5 == 0 && LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(
                            NPC,
                            NPC.Center,
                            NPC.velocity.SafeNormalize(Vector2.Zero) * 5,
                            ProjectileType<IceSpike>(),
                            ai0: 300
                            );
                    }
                }
                break;
            case 120:
                targetPosition = player.Center + Vector2.UnitY * 1000;
                DoDrawFlashlight(Color.Red);
                break;
            case > 60: // Moving below player
                Print("Move below player");
                MoveToPos(player.Center + Vector2.UnitY * 1000, 20, 60);
                break;
            case > 0: // Rising up quickly
                Print("Move above player fast");
                StopDrawingFlashlight();
                MoveToPos(player.Center - Vector2.UnitY * 1000, 20, 45);
                break;
            case 0:
                AttackTimer = 360;
                return;
        }

        AttackTimer--;
    }

    void SineIceBurst()
    {
        switch (AttackTimer)
        {
            case 300:
                MoveToPos(player.Center, 60, 10f);
                break;
            case > 240: // Moving to player slowly
                MoveToPos(player.Center, 150, 20f);
                break;
            case > 60: // Move to player fast (circling), spawn ice burst projectiles in a pattern (l->r, l<-r, l->r)
                MoveToPos(player.Center, 180, 45f);
                if (AttackTimer % 20 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            int direction = AttackCount2 % 2 == 0 ? -1 : 1;
                            float xPosOffset = AttackCount * direction * 360 + Main.rand.NextFloat(-64, 64);
                            float yPosOffset = i * 300 + Main.rand.NextFloat(-40, 40);
                            Vector2 pos = player.Center + new Vector2(xPosOffset, yPosOffset);
                            LemonUtils.QuickProj(
                                NPC,
                                pos,
                                Vector2.Zero,
                                ProjectileType<IceBurst>(),
                                ai0: 60,
                                ai1: 150,
                                ai2: 8
                                );
                        }
                    }
                    AttackCount++;
                }
                break;
            case > 0:
                MoveToPos(player.Center, 180, 40f);
                break;
            case 0:
                AttackTimer = 300;
                AttackCount = -4; // x position offset of ice shards
                AttackCount2++; // how many times attack has been executed
                return;
        }

        AttackTimer--;
    }

    void PlayRoar()
    {
        SoundEngine.PlaySound(SoundID.NPCDeath10 with { PitchRange = (-0.6f, -0.4f) }, NPC.Center);
        SoundEngine.PlaySound(SoundID.Roar with { PitchRange = (-0.6f, -0.4f) }, NPC.Center);
    }

    void Print(string text)
    {
        if (debugText)
        {
            Main.NewText(text);
        }
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        /*
        LeadingConditionRule classicRule = new LeadingConditionRule(new Conditions.NotExpert());
        classicRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PureStardust>(), 1, 4, 8));
        classicRule.OnSuccess(ItemDropRule.Common(ItemID.FragmentStardust, 1, 10, 20));
        classicRule.OnSuccess(ItemDropRule.Common(ItemID.LunarBar, 1, 5, 12));
        npcLoot.Add(classicRule);
        npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<StardustLeviathanBossBag>()));
        */
    }

    public override void BossLoot(ref int potionType)
    {
        potionType = ItemID.GreaterHealingPotion;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        cooldownSlot = ImmunityCooldownID.Bosses;
        return true;
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

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(targetPosition.X);
        writer.Write(targetPosition.Y);
        writer.Write(attackDuration);
        writer.Write(AttackCount2);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        targetPosition.X = reader.ReadSingle();
        targetPosition.Y = reader.ReadSingle();
        attackDuration = reader.ReadSingle();
        AttackCount2 = reader.ReadInt32();
    }

    public override void OnKill()
    {
        /*for (int i = 0; i < 16; i++)
        {
            Gore gore = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)), new Vector2(Main.rand.NextFloat(-5, 5)), Main.rand.Next(61, 64), Main.rand.NextFloat(2f, 5f));
        }*/
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (player == null || !player.Alive())
        {
            return;
        }

        DrawFlashlight();
    }

    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
    }

    void DrawFlashlight()
    {
        Vector2 toPlayer = player.Center - NPC.Center;
        float toPlayerDistance = toPlayer.Length();
        Vector2 toPlayerDir = toPlayer.SafeNormalize(Vector2.Zero);
        Texture2D texture = ParacosmTextures.GlowBallTexture.Value;
        float count = toPlayerDistance / 25;
        for (int i = 0; i < count; i++)
        {
            float segmentSize = 4f * (i + 1) / count;
            float spacing = toPlayerDistance / count;
            Vector2 pos = NPC.Center + toPlayerDir * spacing * i;
            float opacity = (count - i) / count * flashlightOpacity;
            LemonUtils.DrawGlow(pos, flashlightColor, opacity, segmentSize);
        }
    }
}