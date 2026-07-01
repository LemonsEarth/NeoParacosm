using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Weapons.Summon;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.NPCs.Hostile.Corruption;

public class SentientCursedFlame : ModNPC
{
    public override string Texture => ParacosmTextures.Empty100TexPath;

    float AITimer = 0;
    ref float ExplodeTimer => ref NPC.ai[2];
    ref float IdleTimer => ref NPC.ai[3];
    bool doExplode = false;
    int AttackTimer = 0;
    float attackSpeed = 1;
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

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 48;
        NPC.height = 48;
        NPC.lifeMax = 400;
        NPC.defense = 0;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit11 with { PitchRange = (-0.3f, -0.1f) };
        NPC.DeathSound = SoundID.NPCDeath15 with { PitchRange = (-0.3f, -0.1f) };
        NPC.value = 1000;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.5f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        int planteraMul = NPC.downedPlantBoss ? 1 : 1;
        NPC.lifeMax = NPC.lifeMax * planteraMul;
        if (Main.hardMode) NPC.damage = (int)(NPC.damage * 1.25f);
        if (NPC.downedPlantBoss) NPC.damage = (int)(NPC.damage * 1.25f);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            });
    }

    public override void OnKill()
    {
        Vector2 movedPos = Vector2.Lerp(NPC.Center, Main.LocalPlayer.Center, 0.8f);
        SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
        SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
        SoundEngine.PlaySound(SoundID.Item14 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
        LemonUtils.DustBurst(6, NPC.Center, DustType<FireDust>(), 5, 5, 0.3f, 0.6f, Color.Lime);
    }

    public override void AI()
    {
        NPC.TargetClosest(true);
        Dust.NewDustPerfect(
           NPC.RandomPos(),
           DustType<FireDust>(),
           -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f),
           Alpha: 150,
           newColor: Color.Lime,
           Scale: Main.rand.NextFloat(0.3f, 0.6f));

        if (doExplode)
        {
            ExplodeTimer++;
            NPC.velocity *= 0.95f;

            if (ExplodeTimer > 60)
            {
                NPC.SimpleStrikeNPC(9999, 1);
                LemonUtils.DustBurst(8, NPC.Center, DustType<FireDust>(), 8, 8, 0.5f, 1f, Color.Lime);
                if (LemonUtils.NotClient())
                {
                    for (int i = 0; i < 8; i++)
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, Vector2.UnitY.RotatedBy(i * MathHelper.PiOver4) * 2, ProjectileType<CursedFlameSphere>(), damage: NPC.damage * 2, ai1: 1.03f);
                    }
                }
            }
            return;
        }

        if (NPC.HasValidTarget)
        {
            TargetPos = Main.player[NPC.target].Center;
            Vector2 dirToTarget = NPC.Center.DirectionTo(TargetPos);

            NPC.velocity += dirToTarget * 0.05f;

            if (NPC.DistanceSQ(TargetPos) < 100 * 100)
            {
                doExplode = true;
                SoundEngine.PlaySound(SFX.ManaCrystal with { PitchRange = (0.5f, 0.7f), Volume = 0.7f });
                NPC.scale = 1.5f;
                LemonUtils.DustBurst(18, NPC.Center, DustType<FireDust>(), 6, 6, 0.75f, 1.5f, Color.Lime);
                return;
            }
            IdleTimer = 0;
            AttackTimer++;
        }
        else
        {
            if (IdleTimer % 180 == 0)
            {
                if (LemonUtils.NotClient())
                {
                    TargetPos = NPC.Center + Main.rand.NextVector2CircularEdge(500, 500);
                }
                NPC.netUpdate = true;

                Vector2 dirToTarget = NPC.Center.DirectionTo(TargetPos);
                if (NPC.velocity.LengthSquared() < 6 * 6)
                {
                    NPC.velocity += dirToTarget * 0.1f;
                }
            }
            IdleTimer++;
            AttackTimer = 0;
        }

        AITimer++;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = ParacosmTextures.Empty100Tex.Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;

        Vector2 drawPos = NPC.Center - Main.screenPosition;
        var shader = GameShaders.Misc["NeoParacosm:FireShader"];
        shader.UseImage1(ParacosmTextures.NoiseTexture);
        shader.UseColor(Color.Green * NPC.Opacity);
        shader.Shader.Parameters["flameHeightDownward"].SetValue(1); // Higher number lowers the height of the flame
        shader.Shader.Parameters["moveVector"].SetValue(Vector2.UnitY); // Higher number lowers the height of the flame
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        shader.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale * 1f, SpriteEffects.None, 0);
        shader.UseColor(Color.White * NPC.Opacity);
        shader.Shader.Parameters["flameHeightDownward"].SetValue(1f);
        shader.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale * 1f * 0.5f, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        LemonUtils.DrawGlow(NPC.Center, Color.White, NPC.Opacity, NPC.scale);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return (spawnInfo.Player.ZoneCorrupt && (NPC.downedBoss3 || Main.hardMode)) ? 0.1f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<OcculptStaff>(), 15, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        target.AddBuff(BuffID.CursedInferno, 60);
    }
}
