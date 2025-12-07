using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Crimson;

public class CrimsonWalker : ModNPC
{
    float AITimer = 0;
    int hitTimer = 0;

    enum State
    {
        Idle,
        Lunge
    }

    State state = State.Idle;
    bool close = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 54;
        NPC.height = 64;
        NPC.lifeMax = 50;
        NPC.defense = 4;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 100;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.6f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
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
        target.AddBuff(BuffID.Ichor, 180);
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
        Player player = Main.player[NPC.target];
        if (hitTimer > 0) hitTimer--;
        if (Main.rand.NextBool(300))
        {
            SoundEngine.PlaySound(SoundID.Mummy with { PitchRange = (-1f, 1f), Volume = 0.5f }, NPC.Center);
            SoundEngine.PlaySound(SoundID.ZombieMoan with { PitchRange = (-1f, 1f), Volume = 0.5f }, NPC.Center);
        }

        if (NPC.Center.Distance(player.Center) < 500)
        {
            close = true;
        }
        else if (NPC.Center.Distance(player.Center) > 1000)
        {
            close = false;
            AITimer = 0;
        }

        if (close)
        {
            int dashCD = Main.hardMode ? 60 : 90;
            if (AITimer % dashCD == 0 && AITimer > 0)
            {
                if (LemonUtils.NotClient())
                {
                    NPC.velocity += NPC.Center.DirectionTo(player.Center) * Main.rand.NextFloat(10, 20);
                    if (Main.rand.NextBool(3) && Main.hardMode)
                    {
                        NPC.velocity -= Vector2.UnitY * Main.rand.NextFloat(8, 16);
                    }
                }
                NPC.netUpdate = true;
            }
        }

        if (Math.Abs(NPC.velocity.X) < 1)
        {
            state = State.Idle;
        }
        else
        {
            state = State.Lunge;
        }

        NPC.velocity.X /= 1.05f;

        AITimer++;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        close = true;
        if (hitTimer != 0)
        {
            return;
        }
        hitTimer = 60;
        int count = Main.rand.Next(1, 4);

        int projCount = LemonUtils.GetDifficulty();

        for (int i = 0; i < projCount; i++)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(NPC, NPC.RandomPos(), Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 4), ProjectileID.GoldenShowerHostile, NPC.damage / 8);
            }
        }

        for (int i = 0; i < count; i++)
        {
            Gore.NewGore(NPC.GetSource_FromThis(), NPC.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), GoreType<RedGore>());
        }
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 20;
        NPC.frameCounter += 1;
        int startFrame = state == State.Lunge ? 0 : 1;
        int maxFrame = state == State.Lunge ? 0 : 2;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > maxFrame * frameHeight)
            {
                NPC.frame.Y = startFrame * frameHeight;
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
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemID.Ichor, 2, 1, 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        if (NPC.HasValidTarget)
        {
            return Main.player[NPC.target].Center.Y - 16 > NPC.Center.Y;
        }
        return null;
    }
}
