using NeoParacosm.Content.Items.Accessories.Combat.Defensive;
using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using NeoParacosm.Content.Items.Currencies;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Surface;

public class ClericMercenary : ModNPC
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
        NPC.width = 32;
        NPC.height = 48;
        NPC.lifeMax = 300;
        NPC.defense = 15;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 2000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.6f;
    }

    public override void FindFrame(int frameHeight)
    {
        if (attacking)
        {
            if (NPC.frameCounter >= 12 && NPC.frame.Y < frameHeight * 2)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
            }
        }
        else
        {
            if (NPC.frameCounter >= 12 && NPC.frame.Y > 0)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y -= frameHeight;
            }
        }
        NPC.frameCounter++;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return Main.hardMode && (Main.invasionType == InvasionID.PirateInvasion || Main.invasionType == InvasionID.GoblinArmy) ? 0.01f : 0f;
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

    bool attacking = false;
    int attackingTimer = 0;
    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        Lighting.AddLight(NPC.Center, 0.5f, 0.5f, 1f);
        NPC.TargetClosest();
        NPC.spriteDirection = -NPC.direction;
        player = NPC.GetTarget();
        if (attacking)
        {
            Dust.NewDustPerfect(NPC.RandomPos(), DustID.GemTopaz, -Vector2.UnitY * 2).noGravity = true;
            if (attackingTimer == 180)
            {
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickPulse(NPC, NPC.Center, 5f, 2f, 5f, Color.Gold);
                    foreach (var npc in Main.ActiveNPCs)
                    {
                        if (npc.CanBeChasedBy() && !npc.boss)
                        {
                            int healHP = (int)(npc.lifeMax * 0.33f);
                            if (npc.life + healHP > npc.lifeMax)
                            {
                                healHP = npc.lifeMax - npc.life;
                            }

                            npc.life += healHP;
                            npc.HealEffect(healHP);
                        }
                    }

                }
            }
            attackingTimer++;
        }
        else
        {
            attackingTimer = 0;
        }

        if (NPC.GetLifePercent() < 0.5f)
        {
            foreach (var ply in Main.ActivePlayers)
            {
                if (ply.DistanceSQ(NPC.Center) < 400 * 400)
                {
                    ply.AddBuff(BuffID.WitheredArmor, 60 * 10);
                    ply.AddBuff(BuffID.WitheredWeapon, 60 * 10);
                }
            }

            LemonUtils.DustRing(NPC.Center, 400, 8, DustID.GemTopaz, 2);
            LemonUtils.DustRing(NPC.Center, 400, 8, DustID.GemTopaz, -2);
        }

        if (AITimer % 300 == 0 && AITimer > 0)
        {
            attacking = !attacking;
        }

        NPC.velocity.X *= 0.93f;
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
                LemonUtils.DustBurst(12, NPC.Center, DustID.GemTopaz, 5, 5, 2, 4);
                SoundEngine.PlaySound(SoundID.Item8 with { PitchRange = (-0.4f, -0.2f) }, NPC.Center);
                NPC.Center = chosenPos;
                LemonUtils.DustBurst(12, NPC.Center, DustID.GemTopaz, 5, 5, 2, 4);
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
        npcLoot.Add(ItemDropRule.Common(ItemType<HolyBarrier>(), 20));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
