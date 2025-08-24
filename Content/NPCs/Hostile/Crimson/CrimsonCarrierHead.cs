using NeoParacosm.Common.Utils;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class CrimsonCarrierHead : ModNPC
{
    float AITimer = 0;
    float AttackTimer = 0;
    float AttackCount = 0;

    bool spawnedEnemies = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }

    public override void SetDefaults()
    {
        NPC.width = 50;
        NPC.height = 50;
        NPC.lifeMax = 50;
        NPC.defense = 6;
        NPC.damage = 10;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 100;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.6f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        int planteraMul = NPC.downedPlantBoss ? 3 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMul;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {

    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.velocity = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloat(12, 18);
            }
            NPC.netUpdate = true;
        }
        if (AITimer % 20 == 0)
        {
            Vector2 pos = NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height));
            LemonUtils.DustCircle(pos, 8, Main.rand.NextFloat(4, 8), DustID.GemRuby);
        }
        NPC.rotation = MathHelper.ToRadians(NPC.velocity.Length());
        NPC.velocity /= 1.05f;
        if (AITimer > 300)
        {
            NPC.SimpleStrikeNPC(NPC.life, 1);
        }
        AITimer++;
    }

    public override void OnKill()
    {
        if (spawnedEnemies) return;
        for (int i = 0; i < 4; i++)
        {
            Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(3, 8), Main.rand.NextFromList(952, 953, 954, 955, 237, 238, 239, 668, 669, 670, 671, 672));
        }
        spawnedEnemies = true;
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            int count = Main.rand.Next(3, 8);
            for (int i = 0; i < count; i++)
            {
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, NPCType<CrimsonInfectionForm>());
            }
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
