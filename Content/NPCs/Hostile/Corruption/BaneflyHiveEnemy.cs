using NeoParacosm.Common.Utils;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.Corruption;

public class BaneflyHiveEnemy : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 50;
        NPC.height = 50;
        NPC.lifeMax = 150;
        NPC.defense = 8;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 1000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        int planteraMul = NPC.downedPlantBoss ? 10 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMul;
        int planteraMulDF = NPC.downedPlantBoss ? 4 : 1;
        NPC.defense *= planteraMulDF;
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
        Dust.NewDustPerfect(NPC.Center, DustID.Corruption);
        SoundEngine.PlaySound(SoundID.NPCDeath66 with { Volume = 0.2f }, NPC.Center);
    }

    public override void AI()
    {
        NPC.Center += Vector2.UnitY * (float)Math.Sin(AITimer / 24f);
        if (AITimer % 600 == 0)
        {
            if (LemonUtils.NotClient())
            {
                for (int i = 0; i < 4; i++)
                {
                    NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.RandomPos(), NPCType<BaneflyEnemy>(), ai0: NPC.whoAmI);
                }
            }
        }
        Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.Corruption, Scale: 2f).noGravity = true;
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        int chanceBoost = NPC.downedBoss2 ? 4 : 1;
        return (spawnInfo.Player.ZoneCorrupt) ? 0.05f * chanceBoost : 0f;
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
