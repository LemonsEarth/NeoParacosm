using NeoParacosm.Content.Projectiles.Hostile.Evil;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Ice;

public class IceMageStatue : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        NPC.DontDropAnything();
    }

    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 50;
        NPC.lifeMax = 200;
        NPC.defense = 15;
        NPC.damage = 1;
        NPC.HitSound = SoundID.Item50;
        NPC.DeathSound = SoundID.Item27;
        NPC.value = 0;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (Main.getGoodWorld)
        {
            NPC.aiStyle = NPCAIStyleID.Slime;
            AIType = NPCID.SpikedIceSlime;
        }
    }

    public override bool CheckActive()
    {
        return false;
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
        return 0;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {

    }

    public override void AI()
    {

        AITimer++;
    }

    public override void OnKill()
    {
        LemonUtils.DustBurst(12, NPC.Center, DustID.IceTorch, 5, 5, 2, 4);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
