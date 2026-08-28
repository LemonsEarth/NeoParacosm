using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Hostile.Jungle;

public class Herberus : ModNPC
{
    int AITimer = 0;
    bool stationary = false;
    int stationaryTimer = 0;
    List<NPC> heads = new List<NPC>();

    bool reachedPhase2 = false;

    public override void SendExtraAI(BinaryWriter writer)
    {
        for (int i = 0; i < heads.Count; i++)
        {
            writer.Write(heads[i].whoAmI);
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        for (int i = 0; i < heads.Count; i++)
        {
            heads[i] = Main.npc[reader.ReadInt32()];
        }
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 48;
        NPC.lifeMax = 500;
        NPC.defense = 8;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 1000;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.8f;
    }

    void SpawnHeads()
    {
        if (LemonUtils.NotClient())
        {
            heads.Clear();
            for (int i = 1; i <= 3; i++)
            {
                NPC npc = NPC.NewNPCDirect(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    NPCType<HerberusHead>()
                    );

                if (npc.ModNPC is HerberusHead head)
                {
                    head.BodyWhoAmI = NPC.whoAmI;
                    head.HeadNum = i;
                }
                heads.Add(npc);
            }
        }
        NPC.netUpdate = true;
    }

    void GoStationary()
    {
        stationary = true;
        stationaryTimer = 0;
    }


    public override bool PreAI()
    {
        if (AITimer == 0)
        {
            SpawnHeads();
        }
        if (heads.All(npc => !npc.active || npc.life == 0 || npc.type != NPCType<HerberusHead>()) && !stationary)
        {
            GoStationary();
        }

        if (stationary)
        {
            NPC.velocity.X *= 0.8f;
            int stationaryDuration = 480;
            int dustCD = (stationaryDuration - stationaryTimer) / 10 + 1;
            for (int i = 0; i < stationaryTimer / 60; i++)
            {
                Dust.NewDustPerfect(NPC.RandomPos(), DustID.RuneWizard, -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
            }

            if (stationaryTimer >= stationaryDuration)
            {
                SpawnHeads();
                stationary = false;
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(NPC.RandomPos(), DustID.RuneWizard, -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                }
                SoundEngine.PlaySound(ParacosmSFX.DragonRoar with { PitchRange = (-0.5f, -0.3f) }, NPC.Center);
                SoundEngine.PlaySound(ParacosmSFX.DragonRoar with { PitchRange = (-0.1f, 0.1f) }, NPC.Center);
                SoundEngine.PlaySound(ParacosmSFX.DragonRoar with { PitchRange = (0.2f, 0.4f) }, NPC.Center);
            }
            stationaryTimer++;
            AITimer++;
            return false;
        }

        NPC.TargetClosest();
        NPC.spriteDirection = NPC.direction;

        return true;
    }

    public override void AI()
    {
        //Main.NewText(NPC.spriteDirection);
        if (AITimer == 0)
        {

        }

        if (NPC.GetLifePercent() <= 0.6f && !reachedPhase2 && !stationary)
        {
            reachedPhase2 = true;
            SoundEngine.PlaySound(ParacosmSFX.DragonRoar with { PitchRange = (-0.5f, -0.3f) }, NPC.Center);
            SoundEngine.PlaySound(ParacosmSFX.DragonRoar with { PitchRange = (-0.1f, 0.1f) }, NPC.Center);
            SoundEngine.PlaySound(ParacosmSFX.DragonRoar with { PitchRange = (0.2f, 0.4f) }, NPC.Center);
            for (int i = 0; i < 20; i++)
            {
                Vector2 randVector = Main.rand.NextVector2Circular(10, 10);
                Vector2 randVector2 = Main.rand.NextVector2Circular(3, 3);
                Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.OrangeStainedGlass, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
                Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.GemTopaz, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            }
        }

        if (reachedPhase2 && !stationary)
        {
            Vector2 randVector = Main.rand.NextVector2Circular(4, 4);
            Vector2 randVector2 = Main.rand.NextVector2Circular(1, 1);
            Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.OrangeStainedGlass, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            Dust.NewDustDirect(NPC.RandomPos(), 2, 2, DustID.GemTopaz, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        }

        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        if (stationary)
        {
            NPC.frame.Y = 0;
            return;
        }

        if (NPC.velocity.Y != 0)
        {
            NPC.frame.Y = 3 * frameHeight;
            return;
        }

        int frameDur = 6;
        NPC.frameCounter++;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 3 * frameHeight)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
        }
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        LemonUtils.DustBurst(5, NPC.Center, DustID.RuneWizard, 3, 3, 0.6f, 1f);
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 20; i++)
            {
                ParticleSystem.SpawnParticle(
                ParticleID.Gas,
                NPC.RandomPos(),
                Main.rand.NextVector2Circular(6, 6),
                Color.GreenYellow,
                scale: Main.rand.NextFloat(0.6f, 1f),
                data0: 0.2f
                );
            }
            for (int i = 0; i < 10; i++)
            {
                ParticleSystem.SpawnParticle(
                ParticleID.Gas,
                NPC.RandomPos(),
                Main.rand.NextVector2Circular(6, 6),
                Color.Orange,
                scale: Main.rand.NextFloat(0.6f, 1f),
                data0: 0.2f
                );
            }
            for (int i = 0; i < 10; i++)
            {
                ParticleSystem.SpawnParticle(
                ParticleID.Gas,
                NPC.RandomPos(),
                Main.rand.NextVector2Circular(6, 6),
                Color.OrangeRed,
                scale: Main.rand.NextFloat(0.6f, 1f),
                data0: 0.2f
                );
            }
        }
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        if (stationary)
        {
            modifiers.DisableKnockback();
        }
    }

    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
    {

    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (!stationary)
        {
            modifiers.FinalDamage *= 0.5f;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

    }

    public override bool CheckActive()
    {
        return false;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.Player.ZoneJungle && spawnInfo.Player.ZoneOverworldHeight ? 0.02f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<PureLifeEnergy>(), 1, minimumDropped: 2, maximumDropped: 4));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return NPC.ShouldFallThroughPlatforms(8);
    }
}


