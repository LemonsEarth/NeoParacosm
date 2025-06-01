using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Biomes.TheDeep;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
namespace NeoParacosm.Content.NPCs.Hostile.TheDeepHigh;

public class MarksmanFish : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 2;
    }

    public override void SetDefaults()
    {
        NPC.width = 50;
        NPC.height = 50;
        NPC.lifeMax = 70;
        NPC.defense = 15;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.friendly = false;
        NPC.value = 500;
        NPC.aiStyle = NPCAIStyleID.Piranha;
        AIType = NPCID.Shark;
        NPC.knockBackResist = 0.3f;
        NPC.noGravity = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
            });
    }

    public override void OnKill()
    {
        
    }

    public override void AI()
    {
        NPC.spriteDirection = NPC.direction;
        Player player = Main.player[NPC.target];
        if (NPC.HasPlayerTarget && AITimer % 180 == 0 && NPC.Distance(player.Center) < 1000 && Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2))
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                LemonUtils.QuickProj(NPC, NPC.Center, NPC.DirectionTo(player.Center) * Main.rand.NextFloat(12, 24), ProjectileID.FrostBlastHostile);
            }
        }
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 20;
        NPC.frameCounter += 1;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 1 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.Player.InModBiome<DeepHigh>() && spawnInfo.Water ? 0.1f : 0;

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
