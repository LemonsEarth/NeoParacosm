using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Accessories.Misc;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Meteor;

public class GiantMeteorHead : ModNPC
{
    int AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 64;
        NPC.lifeMax = 150;
        NPC.defense = 10;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit3;
        NPC.DeathSound = SoundID.NPCDeath3;
        NPC.value = 3000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.2f;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
        NPC.aiStyle = NPCAIStyleID.Flying;
        AIType = NPCID.MeteorHead;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Meteor,
            });
    }

    public override void OnKill()
    {
        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.MeteorHead, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.GemTopaz, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        }
    }

    public override void AI()
    {
        if (AITimer % 300 == 0 && AITimer > 0)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(
                    NPC,
                    NPC.Center,
                    Vector2.Zero,
                    ProjectileType<GravitySuckyProj>(),
                    0,
                    ai0: 400,
                    ai1: 40,
                    ai2: 3
                    );
            }
        }
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 12;
        NPC.frameCounter += 1;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 3 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.Player.ZoneMeteor ? 0.1f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<MeteorFragments>(), 1, minimumDropped: 50, maximumDropped: 70));
        npcLoot.Add(ItemDropRule.NormalvsExpert(ItemType<GravityRing>(), 10, 5));
        npcLoot.Add(ItemDropRule.NormalvsExpert(ItemType<GravityFieldSpell>(), 15, 10));
        npcLoot.Add(ItemDropRule.Common(ItemID.Meteorite, 1, minimumDropped: 5, maximumDropped: 10));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
