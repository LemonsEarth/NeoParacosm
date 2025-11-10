using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Projectiles.Hostile;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.Corruption;

public class CorruptMage : ModNPC
{
    int AITimer = 0;
    int AttackTimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 44;
        NPC.height = 48;
        NPC.lifeMax = 100;
        NPC.defense = 12;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 1000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.5f;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        int planteraMul = NPC.downedPlantBoss ? 3 : 1;
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
        NPC.TargetClosest(true);
        NPC.spriteDirection = -NPC.direction;
        NPC.Center += Vector2.UnitY * (float)Math.Sin(AITimer / 24f) * 0.5f;
        if (NPC.HasValidTarget)
        {
            Player player = Main.player[NPC.target];
            Vector2 abovePos = player.Center - Vector2.UnitY * 300;
            Vector2 belowOffset = Vector2.UnitY * 150;
            NPC.MoveToPos(abovePos, Main.rand.NextFloat(0.06f, 0.2f), Main.rand.NextFloat(0.06f, 0.2f), Main.rand.NextFloat(0.02f, 0.1f), Main.rand.NextFloat(0.02f, 0.1f));

            if (AttackTimer >= 300 && AttackTimer <= 600 && AttackTimer % 60 == 0)
            {
                if (LemonUtils.NotClient())
                {
                    float randomAngle = MathHelper.ToRadians(Main.rand.NextFloat(-45, 45));
                    Vector2 belowPos = !Main.hardMode ? player.Center + belowOffset : player.Center + belowOffset.RotatedBy(randomAngle);
                    LemonUtils.DustCircle(belowPos, 8, 8, DustID.Corruption, scale: 2f);
                    Vector2 dir = belowPos.DirectionTo(player.Center);
                    LemonUtils.QuickProj(NPC, belowPos, dir * 20, ProjectileType<VilethornHostile>());
                }
            }

            AttackTimer++;
            if (AttackTimer > 600) AttackTimer = 0;
        }
        //Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.Corruption, Scale: 3f).noGravity = true;
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        int chanceBoost = NPC.downedBoss2 ? 4 : 1;
        return (spawnInfo.Player.ZoneCorrupt) ? 0.02f * chanceBoost : 0f;
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
