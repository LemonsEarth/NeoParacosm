using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class RotPerfumeValve : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 28;
        NPC.lifeMax = 50;
        NPC.defense = 6;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 300;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.Crab;
        NPC.knockBackResist = 0.4f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.3f);
        int planteraMul = NPC.downedPlantBoss ? 3 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMul;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {

    }

    public override void AI()
    {
        NPC.TargetClosest(true);

        Player player = Main.player[NPC.target];

        /*if (Main.rand.NextBool(300))
        {
            SoundEngine.PlaySound(SoundID.Mummy with { PitchRange = (-1f, 1f) });
            SoundEngine.PlaySound(SoundID.ZombieMoan with { PitchRange = (-1f, 1f) });
        }*/

        int cd = NPC.downedPlantBoss ? 30 : 60;
        if (AITimer % cd == 0)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(NPC, NPC.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(2, 8), ProjectileType<RotGasHostile>());
            }
        }

        AITimer++;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        Gore.NewGore(NPC.GetSource_FromThis(), NPC.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), GoreType<RedGore>());
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 10;
        NPC.frameCounter += 1;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 3 * frameHeight)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        int chanceBoost = NPC.downedBoss2 ? 2 : 1;
        return (spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight) ? 0.15f * chanceBoost : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 0, maximumDropped: 2));
        npcLoot.Add(ItemDropRule.Common(ItemType<RotPerfume>(), 20, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        //if (NPC.HasValidTarget)
        //{
        //    return Main.player[NPC.target].Center.Y - 16 > NPC.Center.Y;
        //}
        return null;
    }
}
