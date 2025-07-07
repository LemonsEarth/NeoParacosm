using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Projectiles.Hostile;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Corruption;

public class CorruptWalker : ModNPC
{
    float AITimer = 0;
    bool wasHit = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 2;
    }

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 50;
        NPC.lifeMax = 50;
        NPC.defense = 8;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 1000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = false;
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

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        wasHit = true;
    }

    public override void OnKill()
    {
        Dust.NewDustPerfect(NPC.Center, DustID.Corruption);
        SoundEngine.PlaySound(SoundID.PlayerKilled with { Volume = 0.6f }, NPC.Center);
        SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.6f }, NPC.Center);

        for (int i = 0; i < 6; i++)
        {
            LemonUtils.QuickProj(NPC, NPC.RandomPos(), Main.rand.NextVector2Circular(3, 3), ModContent.ProjectileType<DecayGasHostile>());
        }

        for (int i = 0; i < 4; i++)
        {
            NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.RandomPos(), ModContent.NPCType<BaneflyEnemy>(), ai0: NPC.whoAmI);
        }
    }

    public override void AI()
    {
        NPC.TargetClosest(true);
        NPC.spriteDirection = -NPC.direction;

        if (AITimer > 600 && !wasHit)
        {
            NPC.velocity.X = -NPC.velocity.X;
            if (NPC.velocity.X == 0) NPC.velocity.X = 3;
        }
        if (wasHit)
        {
            if (AITimer % 30 == 0)
            {
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.velocity += NPC.Center.DirectionTo(Main.player[NPC.target].Center) * Main.rand.NextFloat(30, 40);
                }
                NPC.netUpdate = true;
            }
        }

        Main.NewText(AITimer);

        if (AITimer > 1200)
        {
            NPC.SimpleStrikeNPC(9999, 1);
        }

        if (NPC.HasValidTarget)
        {
            if (Main.player[NPC.target].Distance(NPC.Center) < 50)
            {
                NPC.SimpleStrikeNPC(9999, 1);
            }
        }

        AITimer++;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ) return;
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 5; i++)
            {
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), ModContent.GoreType<CorruptWalkerGore>());
            }
        }
    }

    public override void FindFrame(int frameHeight)
    {
        if (NPC.velocity.Length() > 5)
        {
            NPC.frame.Y = frameHeight;
        }
        else
        {
            NPC.frame.Y = 0;
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        int chanceBoost = NPC.downedBoss2 ? 4 : 1;
        return (spawnInfo.Player.ZoneCorrupt) ? 0.04f * chanceBoost : 0f;
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
