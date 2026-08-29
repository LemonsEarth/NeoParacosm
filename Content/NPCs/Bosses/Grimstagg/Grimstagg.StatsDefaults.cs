using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Items.Armor.Generic.DeathKnight;
using NeoParacosm.Content.Items.BossBags;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;
using NeoParacosm.Content.Projectiles.Hostile.Death.DeathKnightCaptain;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Bosses.Grimstagg;

// This boss is spread across multiple files
// This file essential ModNPC overrides and loading assets

public partial class Grimstagg : ModNPC
{
    static Asset<Texture2D> HeadTex;
    static Asset<Texture2D> BodyTex;
    static Asset<Texture2D> GrimBallTex;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { };
        /*{
            PortraitScale = 0.2f,
            PortraitPositionYOverride = -150
        };*/
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        HeadTex = Request<Texture2D>(Texture + "Head");
        BodyTex = Request<Texture2D>(Texture + "Body");
        GrimBallTex = Request<Texture2D>(Texture + "Ball");
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
        {
            //BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            //new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Bestiary")),
        });
    }

    public override void SetDefaults()
    {
        NPC.width = 110;
        NPC.height = 110;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.Opacity = 1f;
        NPC.lifeMax = 120000;
        NPC.defense = 60;
        NPC.damage = 100;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 200000;
        NPC.noTileCollide = false;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 10;
        NPC.SpawnWithHigherTime(30);
        //NPC.BossBar = GetInstance<DeathKnightCaptainBossBar>();
        SpawnModBiomes = [DeadForestBiome.BiomeID];

        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Common/Assets/Audio/Music/DeathOrder");
        }
    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.NP().SetDamageReductions(
            (DamageClass.Melee, 10f),
            (DamageClass.Ranged, 15f),
            (DamageClass.Magic, 25f),
            (DamageClass.Summon, 33f),
            (DamageClass.SummonMeleeSpeed, 10f)
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

    public override bool CheckDead()
    {

        return true;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return NPC.dontTakeDamage ? false : null;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        cooldownSlot = ImmunityCooldownID.Bosses;
        return !NPC.dontTakeDamage;
    }

    public override void OnKill()
    {
        //DownedBossSystem.downedDeathKnightCaptain = true;
        //NPC.SetEventFlagCleared(ref DownedBossSystem.downedDeathKnightCaptain, -1);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        /*LeadingConditionRule classicRule = new LeadingConditionRule(new Conditions.NotExpert());
        classicRule.OnSuccess(ItemDropRule.FewFromOptions(2, 1, ItemType<DeathKnightHelmet>(), ItemType<DeathKnightChestplate>(), ItemType<DeathKnightGreaves>()));
        classicRule.OnSuccess(ItemDropRule.Common(ItemType<KnightsLostSoul>(), 1, 10, 16));
        //classicRule.OnSuccess(ItemDropRule.Common(ItemType<DivineFlesh>(), 1, 18, 30));
        npcLoot.Add(classicRule);
        npcLoot.Add(ItemDropRule.BossBag(ItemType<DeathKnightTreasureBag>()));
        //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<DreadlordRelicItem>()));*/
    }

    public override void BossLoot(ref int potionType)
    {
        potionType = ItemID.GreaterHealingPotion;
    }
}
