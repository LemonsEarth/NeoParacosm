using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.GoblinArmy;

public class SentientSawblade : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        //NPCID.Sets.slop
    }

    public override void SetDefaults()
    {
        NPC.width = 34;
        NPC.height = 34;
        NPC.lifeMax = 80;
        NPC.defense = 15;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.Item14;
        NPC.value = 1000;
        NPC.knockBackResist = 0f;
        NPC.aiStyle = NPCAIStyleID.BlazingWheel;
        AIType = NPCID.BlazingWheel;
        NPC.noGravity = true;
        NPC.behindTiles = true;
        NPC.scale = 1.2f;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return true;
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return Main.invasionType == InvasionID.GoblinArmy ? 0.05f : 0f;
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

    public override void OnSpawn(IEntitySource source)
    {

    }

    public override bool PreAI()
    {
        return true;
    }

    public override void AI()
    {
        NPC.spriteDirection = LemonUtils.Sign(NPC.velocity.X, 1);
        AITimer++;
    }

    public override void OnKill()
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
