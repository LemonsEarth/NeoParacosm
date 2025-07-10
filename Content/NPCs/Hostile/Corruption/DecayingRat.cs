using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Projectiles.Hostile;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Corruption;

public class DecayingRat : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 42;
        NPC.lifeMax = 60;
        NPC.defense = 4;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath4;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.DesertBeast;
        NPC.value = 500;
        NPC.knockBackResist = 1f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
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

    public override bool PreAI()
    {
        NPC.TargetClosest(true);
        NPC.spriteDirection = -NPC.direction;

        if (Main.tile[NPC.Center.ToTileCoordinates()].HasTile)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.Corruption, Scale: 1.2f);
            }
        }

        if (NPC.HasValidTarget)
        {
            Player player = Main.player[NPC.target];
            Vector2 TileCheckPos = NPC.Center + Vector2.UnitX * NPC.direction * (NPC.width / 2);
            bool TileCheck = Collision.SolidTiles(TileCheckPos, 2, 2);
            if (Collision.CanHitLine(NPC.Center, 16, 16, player.Center, 16, 16) && !TileCheck)
            {
                NPC.noTileCollide = false;
                NPC.noGravity = false;
                return true;
            }
            else
            {
                NPC.velocity = NPC.Center.DirectionTo(player.Center) * 4;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                SoundEngine.PlaySound(SoundID.WormDig with { PitchRange = (-0.2f, 0.2f) }, NPC.Center);
                return false;
            }
        }       

        NPC.noTileCollide = false;
        NPC.noGravity = false;
        return true;
    }

    public override void AI()
    {
        NPC.TargetClosest(true);
        NPC.spriteDirection = -NPC.direction;
        if (AITimer % 120 == 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 dir = -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15, 15)));
                LemonUtils.QuickProj(NPC, NPC.RandomPos(), dir * Main.rand.NextFloat(2, 6), ModContent.ProjectileType<CursedDecayFire>());
            }
        }
        if (Main.rand.NextBool(100))
        {
            SoundEngine.PlaySound(SoundID.Zombie15 with { PitchRange = (-0.2f, 0.2f), Volume = 0.5f }, NPC.Center);
        }
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 15;
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
        int chanceBoost = NPC.downedBoss2 ? 4 : 1;
        return (spawnInfo.Player.ZoneCorrupt) ? 0.06f * chanceBoost : 0f;
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
