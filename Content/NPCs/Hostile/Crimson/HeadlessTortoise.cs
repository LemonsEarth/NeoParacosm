using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class HeadlessTortoise : ModNPC
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 7;
    }

    public override void SetDefaults()
    {
        NPC.width = 32;
        NPC.height = 32;
        NPC.lifeMax = 3000;
        NPC.defense = 40;
        NPC.damage = 60;
        NPC.HitSound = SoundID.NPCHit24 with { PitchRange = (-1f, 1f) };
        NPC.DeathSound = SoundID.NPCDeath27;
        NPC.value = 15000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.1f;
        Banner = Item.NPCtoBanner(NPCID.GiantTortoise);
        BannerItem = Item.BannerToItem(Banner);
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = frameHeight * 3;
        return;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (DarkCataclysmSystem.DarkCataclysmActive && spawnInfo.Player.ZoneCrimson)
        {
            return 0.1f;
        }
        if (spawnInfo.Player.ZoneCrimson && Main.hardMode && spawnInfo.Player.ZoneJungle)
        {
            return 0.2f;
        }
        return 0;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.4f);
        int planteraMulHP = NPC.downedPlantBoss ? 2 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMulHP;
        int planteraMulDF = NPC.downedPlantBoss ? 2 : 1;
        NPC.defense *= planteraMulDF;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        NPC.TargetClosest(false);

        if (!NPC.HasValidTarget)
        {
            AITimer++;
            NPC.velocity.X *= 0.95f;
            NPC.velocity.Y += 0.1f;
            NPC.rotation = 0;
            return;
        }

        Player player = Main.player[NPC.target];
        float distanceToPlayer = NPC.Center.Distance(player.Center);
        float velocityLength = NPC.velocity.Length();
        if (velocityLength > 1)
        {
            NPC.rotation = MathHelper.ToRadians(LemonUtils.Sign(NPC.velocity.X, 1) * AITimer * velocityLength);
        }
        if (Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2) && distanceToPlayer < 500)
        {
            if (NPC.collideY || NPC.collideX)
            {
                NPC.velocity = NPC.DirectionTo(player.Center) * 30;
            }
        }
        else if (distanceToPlayer < 500)
        {
            Dust.NewDustDirect(NPC.RandomPos(10, 10), 2, 2, DustID.Torch, Scale: 3f).noGravity = true;
            if (LemonUtils.NotClient() && AITimer % 240 == 0)
            {
                LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ProjectileType<BombExplosion>(), 100, ai1: 4);
            }
        }
        else
        {
            NPC.velocity.X *= 0.95f;
            NPC.velocity.Y += 0.1f;
            //NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 1 / 5f);
        }

        AITimer++;
    }

    public override void OnKill()
    {

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAfterimages(drawColor, 0.5f);
        return true;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
        npcLoot.Add(ItemDropRule.Common(ItemType<HeadlessTortoiseShell>(), 10, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
