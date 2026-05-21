using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using NeoParacosm.Content.Items.Currencies;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Content.Projectiles.Hostile.Ice;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Surface;

public class DragonHunter : ModNPC
{
    int AITimer = 0;
    int tpTimer = 0;
    Player player;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 50;
        NPC.height = 64;
        NPC.lifeMax = 300;
        NPC.defense = 15;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.value = 10000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.6f;
    }

    public override void FindFrame(int frameHeight)
    {
        if (AITimer > 90)
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
        return spawnInfo.Player.ZoneOverworldHeight && NPC.downedBoss3 ? 0.01f : 0f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            });
    }

    bool fired = false;
    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        Lighting.AddLight(NPC.Center, 0.5f, 0.5f, 1f);
        NPC.TargetClosest(true);
        NPC.spriteDirection = NPC.direction;

        if (!NPC.HasValidTarget)
        {
            AITimer++;
            return;
        }

        player = Main.player[NPC.target];

        if (NPC.Center.Distance(player.Center) < 500 && AITimer >= 90 && !fired)
        {
            SoundEngine.PlaySound(SoundID.Item8 with { PitchRange = (-0.2f, 0.2f) }, NPC.Center);
            fired = true;
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(
                   NPC,
                   player.Center - Vector2.UnitY * 800,
                   Vector2.Zero,
                   ProjectileType<LightningWarningProj>(),
                   ai1: 120,
                   ai2: 1600
                   );
            }
        }


        NPC.velocity.X *= 0.93f;

        if (AITimer >= 120)
        {
            AITimer = 0;
            fired = false;
        }

        if (tpTimer > 0)
        {
            tpTimer--;
        }

        AITimer++;
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        int tpCooldown = 360;
        if (tpTimer == 0 && player != null && player.IsAlive())
        {
            Vector2 chosenPos = NPC.FindSafeTeleportPosition(player.Center, 500, 100, 100);
            if (chosenPos != Vector2.Zero)
            {
                LemonUtils.DustBurst(12, NPC.Center, DustID.GemRuby, 5, 5, 2, 4);
                SoundEngine.PlaySound(SoundID.Item8 with { PitchRange = (-0.4f, -0.2f) }, NPC.Center);
                NPC.Center = chosenPos;
                LemonUtils.DustBurst(12, NPC.Center, DustID.GemRuby, 5, 5, 2, 4);
                SoundEngine.PlaySound(SoundID.Item8 with { PitchRange = (-0.4f, -0.2f) }, NPC.Center);
                tpTimer = tpCooldown;
            }
        }
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnKill()
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.NormalvsExpert(ItemType<IceCometStaff>(), 2, 1));
        npcLoot.Add(ItemDropRule.Common(ItemType<DragonSoul>(), 3, minimumDropped: 2, maximumDropped: 4));
        npcLoot.Add(ItemDropRule.Common(ItemType<LightningStrikeSpell>(), 15));
        npcLoot.Add(ItemDropRule.Common(ItemType<ClanSigil>(), 8));
        //npcLoot.Add(ItemDropRule.Common(ItemID.IceSkates, 5, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
