using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.BloodMoon;

public class BloodArtist : ModNPC
{
    int AITimer = 0;
    int AttackTimer = 0;

    Vector2 TargetPos
    {
        get
        {
            return new Vector2(NPC.ai[0], NPC.ai[1]);
        }
        set
        {
            NPC.ai[0] = value.X;
            NPC.ai[1] = value.Y;

        }
    }

    public static HashSet<int> WeakKillableNPCTypes = new HashSet<int>();
    public static HashSet<int> StrongKillableNPCTypes = new HashSet<int>();
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3;
        WeakKillableNPCTypes = [
            NPCID.BloodZombie, NPCID.Drippler, NPCID.CorruptBunny, NPCID.CrimsonBunny, NPCID.CorruptPenguin, NPCID.CrimsonPenguin,
            NPCID.CorruptGoldfish, NPCID.CrimsonGoldfish, NPCID.PossessedArmor, NPCID.Zombie, NPCID.DemonEye, NPCID.DemonEyeOwl, NPCID.Raven
            ];
        StrongKillableNPCTypes = [
            NPCID.WanderingEye, NPCID.BloodEelHead, NPCID.ZombieMerman, NPCID.GoblinShark, NPCID.EyeballFlyingFish, NPCID.Wraith,
            NPCID.Werewolf, NPCID.Clown
            ];
    }

    public override void SetDefaults()
    {
        NPC.width = 31;
        NPC.height = 62;
        NPC.lifeMax = 500;
        NPC.defense = 8;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 2 * 100 * 100;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.25f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
            });
    }

    public override void OnKill()
    {

    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            LemonUtils.ParticleBurst(
                50,
                NPC.Center,
                ParticleID.Gas,
                10f, 10f,
                0.6f, 1.5f,
                Color.DarkRed
                );
        }
        else
        {
            Dust.NewDustDirect(NPC.RandomPos(-8, -8), 2, 2, DustID.GemRuby);
        }
    }


    public override void AI()
    {
        NPC.TargetClosest(true);
        NPC.spriteDirection = NPC.direction;
        if (!NPC.HasValidTarget)
        {
            AITimer++;
            return;
        }

        Player player = NPC.GetTarget();
        if (AITimer % 360 == 0 || NPC.DistanceSQ(TargetPos) <= 64 * 64)
        {
            if (LemonUtils.NotClient())
            {
                TargetPos = NPC.FindSafeTeleportPosition(player.Center, 400, 200);
            }
            NPC.netUpdate = true;
        }

        foreach (var p in Main.ActivePlayers)
        {
            if (p.DistanceSQ(NPC.Center) < 400 * 400)
            {
                p.AddBuff(BuffType<Bloodboil>(), 300);
            }
        }

        if (Main.expertMode && AITimer % 600 == 0)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickPulse(NPC, NPC.Center, 3f, 15f, 5f, Color.DarkRed, NPC);
            }
            SoundEngine.PlaySound(SFX.WallOfFleshDeath with { PitchRange = (-0.6f, -0.4f) }, NPC.Center);
            SoundEngine.PlaySound(SFX.WallOfFleshDeath with { PitchRange = (0.3f, 0.4f) }, NPC.Center);
            int count = 0;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (count >= 20) break;
                Vector2 dir = npc.DirectionTo(NPC.Center);
                if (WeakKillableNPCTypes.Contains(npc.type))
                {
                    if (LemonUtils.NotClient())
                    {
                        npc.StrikeInstantKill();
                        LemonUtils.QuickProj(
                            NPC,
                            npc.Center,
                            dir * 4,
                            ProjectileType<LostSoulHostile>(),
                            ai0: 60,
                            ai1: 180
                            );
                    }
                    count++;
                }
                else if (StrongKillableNPCTypes.Contains(npc.type))
                {
                    npc.StrikeInstantKill();
                    if (LemonUtils.NotClient())
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            LemonUtils.QuickProj(
                                NPC,
                                npc.Center,
                                dir.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f)) * Main.rand.NextFloat(3, 4),
                                ProjectileType<LostSoulHostile>(),
                                ai0: 90,
                                ai1: 300
                                );
                        }
                    }
                    count++;
                }
            }
        }

        if (TargetPos != Vector2.Zero)
        {
            NPC.MoveToPos(TargetPos, 0.04f, 0.04f, 0.1f, 0.1f);
        }
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.StandardAnimation(12, frameHeight);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return Main.hardMode && Main.bloodMoon && spawnInfo.Player.ZoneOverworldHeight && (NPC.CountNPCS(Type) == 0) ? 0.03f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<ArcaneLight>(), 3, 1, 1));
        npcLoot.Add(ItemDropRule.Common(ItemID.BloodMoonStarter, 3, 1, 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
