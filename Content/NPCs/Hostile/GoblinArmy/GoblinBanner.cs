using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.GoblinArmy;

public class GoblinBanner : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 32;
        NPC.height = 48;
        NPC.lifeMax = 300;
        NPC.defense = 10;
        NPC.damage = 10;
        NPC.HitSound = SoundID.DD2_SkeletonHurt with { PitchRange = (-0.5f, -0.3f) };
        NPC.DeathSound = SoundID.DD2_SkeletonHurt with { PitchRange = (-0.5f, -0.3f) };
        NPC.value = 100;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return Main.invasionType == InvasionID.GoblinArmy ? 0.1f : 0f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions.Goblins,
            });
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        Dust.NewDustPerfect(NPC.RandomPos(), DustID.GemTopaz, -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(0.5f, 0.8f)).noGravity = true;
        foreach (var player in Main.ActivePlayers)
        {
            if (player.IsAlive() && player.DistanceSQ(NPC.Center) < 800 * 800)
            {
                player.AddBuff(BuffType<GoblinDebuff>(), 60);
            }
        }

        AITimer++;
    }

    public override void OnKill()
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Wood, minimumDropped: 20, maximumDropped: 40));

    }

    public override bool? CanFallThroughPlatforms()
    {
        return false;
    }
}
