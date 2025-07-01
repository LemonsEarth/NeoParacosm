using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.Accessories.Misc;
using NeoParacosm.Content.Projectiles.Hostile;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Special;

public class Marauder : ModNPC
{
    float AITimer = 0;
    int AttackTimer = 0;
    Vector2 pos = Vector2.Zero;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 86;
        NPC.lifeMax = 3000;
        NPC.defense = 30;
        NPC.damage = 80;
        NPC.HitSound = SoundID.DD2_SkeletonDeath;
        NPC.DeathSound = SoundID.DD2_SkeletonSummoned;
        NPC.value = 100000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.hide = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        int planteraMul = NPC.downedPlantBoss ? 4 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMul;

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

    }

    public override void HitEffect(NPC.HitInfo hit)
    {

    }

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    int CarrierProj => WorldGen.crimson ? ModContent.ProjectileType<CrimsonCarrierProj>() : ModContent.ProjectileType<CorruptCarrierProj>();
    int stage => NPC.GetLifePercent() != 0 ? 4 - (int)Math.Ceiling(NPC.GetLifePercent() / 0.25f) : 3;
    public override void AI()
    {
        NPC.TargetClosest(true);

        if (!NPC.HasValidTarget)
        {
            AITimer++;
            return;
        }

        Player player = Main.player[NPC.target];

        if (AITimer < 540)
        {
            NPC.Opacity = 0;
            NPC.dontTakeDamage = true;
            pos = player.Center;
            Vector2 randomPos = pos + Main.rand.NextVector2Circular(100, 100);
            LemonUtils.DustCircle(randomPos, 8, 10, DustID.GemRuby, 1.5f);
            SoundEngine.PlaySound(SoundID.NPCDeath6 with { PitchRange = (0, 0.5f) }, player.Center);
        }

        if (AITimer == 600)
        {
            NPC.Opacity = 1;
            NPC.dontTakeDamage = false;
            NPC.Center = pos;
            SoundEngine.PlaySound(SoundID.Zombie105 with { PitchRange = (0, 0.5f) }, NPC.Center);
            LemonUtils.DustCircle(pos, 8, 10, DustID.GemRuby, 2.5f);
        }

        if (AITimer > 720)
        {
            switch (AttackTimer)
            {
                case < 360:
                    NPC.velocity = (player.Center - NPC.Center) / (36f * (stage + 1));
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.ToRadians(15) * NPC.direction, 1f / 30f);
                    if (AttackTimer % (60 / (stage + 1)) == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = -1; i <= 1; i++)
                            {
                                Vector2 dir = NPC.DirectionTo(player.Center).RotatedBy(i * MathHelper.ToRadians(30));
                                LemonUtils.QuickProj(NPC, NPC.Center, dir * 2 * (stage + 1), CarrierProj, NPC.damage / 4);
                            }
                        }
                    }
                    break;
                case < 900:
                    NPC.rotation = MathHelper.ToRadians(AITimer * 12);
                    if (AttackTimer % (60 / (stage + 1)) == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<RotGasHostile>(), NPC.damage / 4);
                            }
                        }
                    }
                    NPC.MoveToPos(player.Center, 0.05f * (stage + 1), 0.05f * (stage + 1), 0.15f, 0.15f);
                    break;
                default:
                    AttackTimer = 0;
                    NPC.rotation = 0;
                    break;
            }
            AttackTimer++;
            if (AITimer % stage + 4 == 0)
            {
                var dust = Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.TintableDust, newColor: new Color(0f, 0f, 0f, 1f), Scale: 3f);
                dust.noGravity = true;
            }
            Lighting.AddLight(NPC.Center, 3, 0, 0);
        }

        AITimer++;
    }

    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsOverPlayers.Add(index);
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = stage * frameHeight;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return (spawnInfo.Player.HasBuff(ModContent.BuffType<ProvokedPresenceDebuff>()) && Main.hardMode) ? 0.05f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullOfAvarice>(), minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }
}
