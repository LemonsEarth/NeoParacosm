using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class RotHornet : ModNPC
{
    int AITimer = 0;
    int AttackTimer = 0;
    bool doBurst = false;

    Vector2 targetPos;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 2;
    }

    public override void SetDefaults()
    {
        NPC.width = 48;
        NPC.height = 46;
        NPC.lifeMax = 400;
        NPC.defense = 12;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 5000;
        NPC.aiStyle = NPCAIStyleID.Flying;
        AIType = NPCID.MossHornet;
        NPC.knockBackResist = 0.5f;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
        Banner = Item.NPCtoBanner(NPCID.MossHornet);
        BannerItem = Item.BannerToItem(Banner);
    }

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    public override void OnSpawn(IEntitySource source)
    {

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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
    }

    public override void OnKill()
    {
        Dust.NewDustPerfect(NPC.Center, DustID.Crimson);
        //SoundEngine.PlaySound(SoundID.NPCDeath66 with { Volume = 0.2f }, NPC.Center);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {

    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {

    }

    public override bool PreAI()
    {

        return true;
    }

    public override void AI()
    {
        NPC.direction = LemonUtils.Sign(NPC.velocity.X, 1);
        NPC.spriteDirection = NPC.direction;

        if (AITimer > 300 && NPC.HasValidTarget)
        {
            doBurst = true;
        }

        if (doBurst)
        {
            if (AttackTimer % 10 == 0 && AttackTimer < 60 && NPC.HasPlayerTarget)
            {
                SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(
                        NPC,
                        NPC.Center,
                        NPC.DirectionTo(NPC.GetTarget().Center) * 5,
                        ProjectileID.Stinger,
                        NPC.damage,
                        5
                        );
                }
            }

            if (AttackTimer >= 60)
            {
                doBurst = false;
                AttackTimer = 0;
                AITimer = 0;
            }
            AttackTimer++;
        }
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 5;
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
        if (DarkCataclysmSystem.DarkCataclysmActive && spawnInfo.Player.ZoneCrimson)
        {
            return 0.1f;
        }
        int chanceBoost = NPC.downedPlantBoss ? 4 : 1;
        return (spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneJungle && Main.hardMode) ?
            0.04f * chanceBoost
            : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Stinger, minimumDropped: 1, maximumDropped: 3));
        npcLoot.Add(ItemDropRule.Common(ItemID.JungleSpores, 2, minimumDropped: 1, maximumDropped: 3));
        npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 4));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }
}
