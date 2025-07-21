using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Projectiles.Hostile;
using NeoParacosm.Core.Systems;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
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
        NPC.width = 200;
        NPC.height = 250;
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
        int frameDur = 12;
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
        return 0;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.4f);
        NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.5f);
    }

    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];

        DespawnCheck();

        if (NPC.life < NPC.lifeMax / 2 && !phase2Reached)
        {
            phaseTransition = true;
            phase2Reached = true;
            AttackTimer = 600;
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

    float wingScale = 1.05f;
    float darkColorBoost = 0f;

    void PhaseTransition()
    {
        switch(AttackTimer)
        {
            case > 0:
                wingScale = MathHelper.Lerp(1.5f, 1f, AttackTimer / 600f);
                darkColorBoost = MathHelper.Lerp(1f, 0f, AttackTimer / 600f);
                break;
            case 0:
                phase = 2;
                phaseTransition = false;
                return;
        }
        AttackTimer--;
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
        Vector2 drawPos = NPC.Center + Vector2.UnitY * 35;
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
