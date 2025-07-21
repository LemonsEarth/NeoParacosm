using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Projectiles.Hostile;
using NeoParacosm.Core.Systems;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.NPCs.Bosses.Deathbird;

[AutoloadBossHead]
public class Deathbird : ModNPC
{
    static Asset<Texture2D> wingTexture;

    ref float AITimer => ref NPC.ai[0];
    public float Attack
    {
        get { return NPC.ai[1]; }
        private set
        {
            int diffMod = -1; // One less attack if not in expert
            if (Main.expertMode)
            {
                diffMod = 0;
            }
            int maxVal = phase == 1 ? 3 : 6;
            if (value > maxVal + diffMod || value < 0)
            {
                NPC.ai[1] = 0;
            }
            else
            {
                NPC.ai[1] = value;
            }
        }
    }
    ref float AttackTimer => ref NPC.ai[2];
    ref float AttackCount => ref NPC.ai[3];

    bool phase2Reached = false;
    int phase = 1;
    bool phaseTransition = false;

    float attackDuration = 0;
    int[] attackDurations = { 480, 480, 900, 1200, 600 };
    int[] attackDurations2 = { 900, 900, 720, 720, 900, 1080, 960 };
    public Player player { get; private set; }
    Vector2 targetPosition = Vector2.Zero;

    public enum Attacks
    {

    }

    public enum Attacks2
    {

    }

    public override void Load()
    {
        wingTexture = ModContent.Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Deathbird/Deathbird_Wings");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { };
        /*{
            PortraitScale = 0.2f,
            PortraitPositionYOverride = -150
        };*/
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Bestiary")),
            });
    }

    public override void SetDefaults()
    {
        NPC.width = 250;
        NPC.height = 200;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.Opacity = 1;
        NPC.lifeMax = 12000;
        NPC.defense = 13;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath52;
        NPC.value = 150000;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 10;
        NPC.SpawnWithHigherTime(30);

        if (!Main.dedServ)
        {
            //Music = MusicLoader.GetMusicSlot(Mod, "Content/Audio/Music/AnotherSamePlace");
        }
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 6;
        NPC.frameCounter += 1;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 4 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!DownedBossSystem.downedDeathbird && !NPC.AnyNPCs(ModContent.NPCType<Deathbird>()))
        {
            return 1f;
        }
        return 0f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.4f);
        NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.5f);
    }

    int INTRO_DURATION = 300;
    int TRANSITION_DURATION = 300;
    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];

        DespawnCheck();

        if (AITimer < INTRO_DURATION)
        {
            Intro();
            AITimer++;
            return;
        }
        if (!phaseTransition) NPC.dontTakeDamage = false;

        if (NPC.life < NPC.lifeMax / 2 && !phase2Reached)
        {
            phaseTransition = true;
            phase2Reached = true;
            AttackTimer = 0;
        }

        if (phaseTransition)
        {
            PhaseTransition();
            AITimer++;
            return;
        }

        AITimer++;
    }

    void SwitchAttacks()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            Attack++;
            if (phase == 1) attackDuration = attackDurations[(int)Attack];
            else attackDuration = attackDurations2[(int)Attack];

            AttackCount = 0;
            AttackTimer = 0;
            NPC.Opacity = 1f;
        }
        NPC.netUpdate = true;
    }

    void DespawnCheck()
    {
        if (player.dead || !player.active || NPC.Center.Distance(player.MountedCenter) > 5000)
        {
            NPC.active = false;
            NPC.life = 0;
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }
    }

    void Intro()
    {
        NPC.dontTakeDamage = true;
        NPC.velocity = Vector2.Zero;
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen);
        }

        if (AITimer < INTRO_DURATION - 120)
        {
            NPC.Opacity = 0f;
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustDirect(NPC.RandomPos(16, 16), 2, 2, DustID.Ash, Scale: Main.rand.NextFloat(1f, 4f)).noGravity = true; ;
            }
        }
        else if (AITimer == INTRO_DURATION - 120)
        {
            NPC.Opacity = 1f;
            for (int i = 0; i < 4; i++)
            {
                LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(1f, 4f));
            }
            PunchCameraModifier mod1 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 90, 1000f, FullName);
            Main.instance.CameraModifiers.Add(mod1);
            SoundEngine.PlaySound(SoundID.Roar with { PitchRange = (-0.1f, 0f)}, NPC.Center);
            SoundEngine.PlaySound(SoundID.Roar with { PitchRange = (0.6f, 0.9f)}, NPC.Center);
            SoundEngine.PlaySound(SoundID.Roar with { PitchRange = (-0.2f, 0.2f)}, NPC.Center);
        }
    }

    float wingScale = 1.05f;
    float darkColorBoost = 0f;
    void PhaseTransition()
    {
        NPC.dontTakeDamage = true;
        switch (AttackTimer)
        {
            case < 120:
                if (AttackTimer % 10 == 0)
                {
                    LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(1f, 4f));
                }
                break;
            case 120:
                wingScale = 1.25f;
                darkColorBoost = 1f;
                LemonUtils.DustCircle(NPC.Center, 24, 24, DustID.GemDiamond, 8f);
                PunchCameraModifier mod1 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 90, 1000f, FullName);
                Main.instance.CameraModifiers.Add(mod1);
                SoundEngine.PlaySound(SoundID.Roar with { PitchRange = (-0.1f, 0f) }, NPC.Center);
                SoundEngine.PlaySound(SoundID.Roar with { PitchRange = (0.6f, 0.9f) }, NPC.Center);
                SoundEngine.PlaySound(SoundID.Roar with { PitchRange = (-0.2f, 0.2f) }, NPC.Center);
                break;
            case 300:
                phase = 2;
                phaseTransition = false;
                return;
        }
        AttackTimer++;
    }

    public override void OnKill()
    {

    }

    public override void HitEffect(NPC.HitInfo hit)
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.Opacity < 1)
        {
            return true;
        }
        var shader = GameShaders.Misc["NeoParacosm:DeathbirdWingShader"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["tolerance"].SetValue(0.5f);
        shader.Shader.Parameters["darkColorBoost"].SetValue(darkColorBoost);
        shader.Shader.Parameters["color"].SetValue(Color.White.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.75f);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Vector2 drawPos = NPC.Center;
        Main.EntitySpriteDraw(wingTexture.Value, drawPos - Main.screenPosition, wingTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, NPC.rotation, wingTexture.Frame(1, 5, 0, 0).Size() * 0.5f, NPC.scale * wingScale, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Main.EntitySpriteDraw(wingTexture.Value, drawPos - Main.screenPosition, wingTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, NPC.rotation, wingTexture.Frame(1, 5, 0, 0).Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        return true;
    }
}
