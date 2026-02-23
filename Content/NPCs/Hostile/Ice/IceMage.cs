using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.NPCs.Hostile.Ice;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using NeoParacosm.Content.Projectiles.Hostile.Ice;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Ice;

public class IceMage : ModNPC
{
    int AITimer = 0;
    int tpTimer = 0;
    Player player;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 2;
    }

    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 50;
        NPC.lifeMax = 150;
        NPC.defense = 10;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.value = 2500;
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
        return spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneRockLayerHeight ? 0.05f : 0f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,
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
        NPC.spriteDirection = -NPC.direction;

        if (!NPC.HasValidTarget)
        {
            AITimer++;
            return;
        }

        player = Main.player[NPC.target];
        if (Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2))
        {
            if (NPC.Center.Distance(player.Center) < 500 && AITimer >= 90 && !fired)
            {
                SoundEngine.PlaySound(SoundID.Item28 with { PitchRange = (-0.2f, 0.2f) }, NPC.Center);
                fired = true;
                if (LemonUtils.NotClient())
                {
                    Vector2 dir = NPC.Center.DirectionTo(player.Center)* Main.rand.NextFloat(3, 5);
                    LemonUtils.QuickProj(NPC, NPC.Center, dir, ProjectileType<IceBlockProjHostile>(), ai0: 300, ai1: player.Center.X, ai2: player.Center.Y);
                }
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
        if (tpTimer == 0 && player != null && player.Alive())
        {
            Vector2 chosenPos = NPC.FindSafeTeleportPosition(player.Center, 500, 100, 100);
            if (chosenPos != Vector2.Zero)
            {
                LemonUtils.DustBurst(12, NPC.Center, DustID.IceTorch, 5, 5, 2, 4);
                SoundEngine.PlaySound(SoundID.Item28 with { PitchRange = (0.6f, 0.8f) }, NPC.Center);
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.position, NPCType<IceMageStatue>());
                NPC.Center = chosenPos;
                SoundEngine.PlaySound(SoundID.Item28 with { PitchRange = (0.6f, 0.8f) }, NPC.Center);
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
        npcLoot.Add(ItemDropRule.NormalvsExpert(ItemType<IceCometStaff>(), 2, 1));
        npcLoot.Add(ItemDropRule.Common(ItemID.IceBlock, minimumDropped: 20, maximumDropped: 50));
        npcLoot.Add(ItemDropRule.Common(ItemID.FlinxFur, 4, minimumDropped: 4, maximumDropped: 8));
        npcLoot.Add(ItemDropRule.Common(ItemID.IceSkates, 5, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
