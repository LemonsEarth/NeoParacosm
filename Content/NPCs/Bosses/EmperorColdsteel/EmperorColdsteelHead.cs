using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Bosses.EmperorColdsteel;

[AutoloadBossHead]
public class EmperorColdsteelHead : ModNPC
{
    int BodyType => ModContent.NPCType<EmperorColdsteelBody>();
    const int MAX_SEGMENT_COUNT = 8;
    public int SegmentCount { get; private set; } = 0;
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
    readonly int[] attackDurations = [600, 1080, 1080, 1080, 1320];
    readonly int[] attackDurations2 = [1600, 1080, 1080, 1080, 1320];

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
        Pillars
    }

    public int Phase { get; private set; } = 0;
    #endregion

    public Player player { get; private set; }
    Vector2 targetPosition = Vector2.Zero;

    List<EmperorColdsteelBody> Segments = new List<EmperorColdsteelBody>();

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

    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        if (AITimer == 0)
        {
            SpawnSegments();
        }
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        player = Main.player[NPC.target];
        NPC.MoveToPos(player.Center, 0.05f, 0.05f, 0.4f, 0.4f);
        DespawnCheck();
        AttackControl();
        AITimer++;
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
        int oldestNPC = latestNPC;
        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0, latestNPC, NPC.whoAmI, SegmentCount);

        Main.npc[oldestNPC].ai[0] = latestNPC; // NPC to follow
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
                case (int)Attacks.BallsNLightning:
                    break;
                case (int)Attacks.MeatballsWithFire:
                    break;
                case (int)Attacks.CursedFlamethrower:
                    break;
                case (int)Attacks.FlameWallsAndSpinning:
                    break;
                case (int)Attacks.DashingWithBalls:
                    break;
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
}