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

public partial class GrimstaggMass : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPC.HideFromBestiary();
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
        NPC.width = 40;
        NPC.height = 40;
        NPC.aiStyle = -1;
        NPC.Opacity = 1f;
        NPC.lifeMax = 120000;
        NPC.defense = 60;
        NPC.damage = 100;
        NPC.hide = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 200000;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 10;
        NPC.SpawnWithHigherTime(30);
        NPC.dontTakeDamage = true;
    }

    public override void OnSpawn(IEntitySource source)
    {

    }

    public override bool CheckActive()
    {
        return false;
    }

    public override bool CheckDead()
    {

        return true;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        //cooldownSlot = ImmunityCooldownID.Bosses;
        //return !NPC.dontTakeDamage;
        return false;
    }

    public override void OnKill()
    {
        //DownedBossSystem.downedDeathKnightCaptain = true;
        //NPC.SetEventFlagCleared(ref DownedBossSystem.downedDeathKnightCaptain, -1);
    }
}
