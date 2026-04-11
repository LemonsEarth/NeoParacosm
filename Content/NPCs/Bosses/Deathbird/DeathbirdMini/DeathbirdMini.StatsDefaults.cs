using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Items.Accessories.Combat;
using NeoParacosm.Core.Systems.Data;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Bosses.Deathbird.DeathbirdMini;

public partial class DeathbirdMini : ModNPC
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
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
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
        NPC.lifeMax = 7500;
        NPC.defense = 7;
        NPC.damage = 40;
        NPC.HitSound = SoundID.DD2_SkeletonHurt;
        NPC.DeathSound = SoundID.NPCDeath52;
        NPC.value = 80000;
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
        if (!DownedBossSystem.downedDeathbirdMini && spawnInfo.Player.InModBiome<DeadForestBiome>() && !NPC.AnyNPCs(NPCType<DeathbirdMini>()))
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

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return !(phase == 1 && Attack == (int)Attacks.Grab);
    }

    public override bool CheckActive()
    {
        return false;
    }

    public override void OnKill()
    {
        DownedBossSystem.downedDeathbirdMini = true;
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
        npcLoot.Add(ItemDropRule.Common(ItemType<RuneOfPeridition>()));
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
}
