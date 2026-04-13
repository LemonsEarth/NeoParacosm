using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Bosses.Dreadlord;

// This boss is spread across multiple files
// This file essential ModNPC overrides and loading assets

public partial class Dreadlord : ModNPC
{
    #region Body Parts
    public DreadlordBodyPart HeadCorrupt { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart HeadCrimson { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart WingCorrupt { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart WingCrimson { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart LegCorrupt { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart LegCrimson { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart Body { get; private set; } = new DreadlordBodyPart();
    public DreadlordBodyPart BackLegs { get; private set; } = new DreadlordBodyPart();

    public static Asset<Texture2D> NeckTextureCorrupt { get; private set; }
    public static Asset<Texture2D> NeckTextureCrimson { get; private set; }
    #endregion

    Dictionary<DamageClass, int> ClassAdaptation = new Dictionary<DamageClass, int>
        {
            {DamageClass.Melee, 0},
            {DamageClass.Ranged, 0},
            {DamageClass.Magic, 0},
            {DamageClass.Summon, 0},
            {DamageClass.Generic, 0},
        };

    public override void Load()
    {
        NeckTextureCorrupt = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordNeckCorrupt");
        NeckTextureCrimson = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordNeckCrimson");
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
                //new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Bestiary")),
            });
    }

    #region Adaptation
    // DamageType adaptation system similar to but AdaptsToDamageTypeNPC
    // TODO: Expand on AdaptsToDamageTypeNPC to allow variables to be configured on a case by case basis

    int maxDamageResistance = 50;
    int damageResistancePerHit = 1;
    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        if (modifiers.DamageType == null) return;
        if (ClassAdaptation.TryGetValue(modifiers.DamageType, out int damageResistance))
        {
            if (damageResistance < maxDamageResistance)
            {
                ClassAdaptation[modifiers.DamageType] += damageResistancePerHit;
            }
            modifiers.FinalDamage *= (100 - damageResistance) / 100f;

            foreach (var key in ClassAdaptation.Keys)
            {
                if (key != modifiers.DamageType)
                {
                    if (ClassAdaptation[key] >= damageResistancePerHit) ClassAdaptation[key] -= damageResistancePerHit;
                }
            }
        }
        else
        {
            if (ClassAdaptation[DamageClass.Generic] < maxDamageResistance)
            {
                ClassAdaptation[DamageClass.Generic] += damageResistancePerHit;
            }
            modifiers.FinalDamage *= (100 - ClassAdaptation[DamageClass.Generic]) / 100f;
        }
    }

    public override void PostAI()
    {
        //foreach(KeyValuePair<DamageClass, int> kvp in ClassAdaptation)
        //{
        //    Main.NewText(kvp.Key + ": " + kvp.Value);
        //}
        if (AITimer % 60 == 0)
        {
            foreach (DamageClass key in ClassAdaptation.Keys)
            {
                if (ClassAdaptation[key] >= damageResistancePerHit * 5)
                {
                    ClassAdaptation[key] -= damageResistancePerHit * 5;
                }
            }
        }
    }
        #endregion

    public override void SetDefaults()
    {
        NPC.width = 284;
        NPC.height = 416;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.Opacity = 1f;
        NPC.lifeMax = 250000;
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

    public override bool CheckDead()
    {
        return true;
    }

    // Disable contact damage when "in background"
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        cooldownSlot = ImmunityCooldownID.Bosses;
        return NPC.scale == 1;
    }

    public override void OnKill()
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override void BossLoot(ref int potionType)
    {
        potionType = ItemID.GreaterHealingPotion;
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }
}
