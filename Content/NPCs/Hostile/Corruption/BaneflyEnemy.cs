using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Corruption;

public class BaneflyEnemy : ModNPC
{
    float AITimer = 0;
    bool foundPlayer;
    ref float ParentIndex => ref NPC.ai[0];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.width = 16;
        NPC.height = 16;
        NPC.lifeMax = 10;
        NPC.defense = 0;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 500;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 1f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        int planteraMul = NPC.downedPlantBoss ? 1 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMul;
        if (Main.hardMode) NPC.damage = (int)(NPC.damage * 1.25f);
        if (NPC.downedPlantBoss) NPC.damage = (int)(NPC.damage * 1.25f);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            });
    }

    public override void OnKill()
    {
        Dust.NewDustPerfect(NPC.Center, DustID.GemRuby);
        SoundEngine.PlaySound(SoundID.NPCDeath66 with { Volume = 0.2f }, NPC.Center);
    }

    public override void AI()
    {
        NPC.TargetClosest(true);
        NPC.spriteDirection = -NPC.direction;
        NPC parent = Main.npc[(int)ParentIndex];

        if (NPC.HasValidTarget && (NPC.Distance(Main.player[NPC.target].Center) < 800 || foundPlayer))
        {
            NPC.MoveToPos(Main.player[NPC.target].Center, Main.rand.NextFloat(0.04f, 0.1f), Main.rand.NextFloat(0.04f, 0.1f), Main.rand.NextFloat(0.1f, 0.2f), Main.rand.NextFloat(0.1f, 0.2f));
            foundPlayer = true;
        }
        else
        {
            if (parent != null && parent.active && parent.type == ModContent.NPCType<BaneflyHiveEnemy>())
            {
                NPC.MoveToPos(parent.Center, Main.rand.NextFloat(0.04f, 0.1f), Main.rand.NextFloat(0.04f, 0.1f), Main.rand.NextFloat(0.1f, 0.2f), Main.rand.NextFloat(0.1f, 0.2f));
            }
            else
            {
                if (AITimer % 20 == 0)
                {
                    NPC.velocity = Main.rand.NextVector2Circular(5, 5);
                }
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
            if (NPC.frame.Y > 3 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return 0f;
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
