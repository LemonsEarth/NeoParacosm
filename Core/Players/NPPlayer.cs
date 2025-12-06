using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Biomes.TheDepths;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using NeoParacosm.Core.Systems;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Runtime.InteropServices;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Core.Players;

public class NPPlayer : ModPlayer
{
    int timer = 0;

    float desaturateEffectOpacity = 0f;
    float desaturateEffectOpacityTimer = 0f;
    float maxDesaturateValue = 0.6f;

    float DCEffectOpacity = 0f;
    float DCEffectOpacityTimer = 0f;
    public override void ResetEffects()
    {

    }

    public override void PostUpdate()
    {
        //LemonUtils.DebugPlayerCenter(Player);
        //Main.NewText("World Surface: " + (int)Main.worldSurface);
        //Main.NewText(WorldDataSystem.ResearcherQuestProgress);
        //WorldDataSystem.ResearcherQuestProgress = WorldDataSystem.ResearcherQuestProgressState.CollectedData;
        /*foreach (var npc in EvilGlobalNPC.EvilEnemiesBonus)
        {
            Main.NewText(ContentSamples.NpcsByNetId[npc]);
        }*/
        //WorldDataSystem.ResearcherQuestProgress = WorldDataSystem.ResearcherQuestProgressState.CollectedData;
        if (NPC.FindFirstNPC(NPCType<Researcher>()) > 0 && Main.npc[NPC.FindFirstNPC(NPCType<Researcher>())].Distance(Player.Center) > 500)
        {
            AscensionUISystem UISystem = GetInstance<AscensionUISystem>();
            UISystem.HideUI();
        }

        /*if (KeybindSystem.CrimsonSacrifice.JustReleased)
        {
            ResearcherUISystem UISystem = GetInstance<ResearcherUISystem>();
            if (UISystem.userInterface.CurrentState == null)
            {
                UISystem.ShowUI();
            }
        }*/
        //Main.NewText(ResearcherQuest.DragonRemainsTileEntityPos - (Player.Center / 16).ToPoint16());
        //Main.NewText(ResearcherQuest.DragonRemainsTileEntityPos);
        timer++;
    }

    public override void PostUpdateMiscEffects()
    {
        DeadForestEffects();

        DepthsEffects();

        DCEffects();

        if (timer % 10 == 0)
        {
            if (!NPC.AnyNPCs(NPCType<Deathbird>()))
            {
                Filters.Scene.Deactivate("NeoParacosm:DeathbirdArenaShader");
            }
        }
    }

    void DeadForestEffects()
    {
        if (Player.InModBiome<DeadForestBiome>())
        {
            desaturateEffectOpacity = MathHelper.Lerp(0, maxDesaturateValue, desaturateEffectOpacityTimer / 60f);
            if (desaturateEffectOpacityTimer < 60) desaturateEffectOpacityTimer++;
            ScreenShaderData data = Filters.Scene.Activate("NeoParacosm:DesaturateShader").GetShader();
            data.UseProgress(desaturateEffectOpacity);

            Player.AddBuff(BuffType<DeadForestDebuff>(), 2);
        }
        else
        {
            desaturateEffectOpacity = MathHelper.Lerp(0, maxDesaturateValue, desaturateEffectOpacityTimer / 60f);
            if (desaturateEffectOpacityTimer > 0) desaturateEffectOpacityTimer--;
            if (desaturateEffectOpacityTimer <= 0)
            {
                Filters.Scene.Deactivate("NeoParacosm:DesaturateShader");
            }
            else
            {
                Filters.Scene["NeoParacosm:DesaturateShader"].GetShader().UseProgress(desaturateEffectOpacity);
            }
        }
    }

    void DepthsEffects()
    {
        if (Player.InModBiome<DepthsHigh>())
        {
            Filters.Scene.Activate("NeoParacosm:ScreenTintShader").GetShader().UseColor(new Color(102, 148, 255));
            Filters.Scene["NeoParacosm:ScreenTintShader"].GetShader().UseProgress(1);
        }
        else
        {
            Filters.Scene.Deactivate("NeoParacosm:ScreenTintShader");
        }
    }

    Vector2 moveT = Vector2.Zero;
    void DCEffects()
    {
        if (ResearcherQuest.DarkCataclysmActive)
        {
            DCEffectOpacity = MathHelper.Lerp(0, 1f, DCEffectOpacityTimer / 60f);

            if (DCEffectOpacityTimer < 60) DCEffectOpacityTimer++;
            ScreenShaderData data = Filters.Scene.Activate("NeoParacosm:DCEffect").GetShader();
            data.UseImage(ParacosmTextures.NoiseTexture.Value);
            data.Shader.Parameters["time"].SetValue(timer / 100f);

            Vector2 pVClamped = Vector2.Clamp(Player.velocity, -Vector2.One * 10, Vector2.One * 10);
            moveT = Vector2.Lerp(moveT, pVClamped, 1f / 60f);
            data.Shader.Parameters["playerVelocity"].SetValue(Player.velocity);
            data.Shader.Parameters["moveT"].SetValue(moveT);
            data.UseProgress(DCEffectOpacity);
            int worldWidth = Main.maxTilesX * 16;
            int worldHeight = Main.maxTilesY * 16;
            int sectionWidth = worldWidth / 16;
            int sectionHeight = worldHeight / 16;
            int sectionX = (int)MathF.Floor(Player.Center.X / sectionWidth);
            int sectionY = (int)MathF.Floor(Player.Center.Y / sectionHeight);
            int biomeDecider = (sectionX + sectionY) % 2;
            if (biomeDecider == 0)
            {
                Player.ZoneCorrupt = true;
            }
            else
            {
                Player.ZoneCrimson = true;
            }

            Lighting.GlobalBrightness = 0.9f;
            if (!SkyManager.Instance["NeoParacosm:DCSky"].IsActive())
            {
                SkyManager.Instance.Activate("NeoParacosm:DCSky");
            }
        }
        else
        {
            DCEffectOpacity = MathHelper.Lerp(0, maxDesaturateValue, DCEffectOpacityTimer / 60f);
            if (DCEffectOpacityTimer > 0) DCEffectOpacityTimer--;
            if (DCEffectOpacityTimer <= 0)
            {
                Filters.Scene.Deactivate("NeoParacosm:DCEffect");
                SkyManager.Instance.Deactivate("NeoParacosm:DCSky");
            }
            else
            {
                Filters.Scene["NeoParacosm:DCEffect"].GetShader().UseProgress(DCEffectOpacity);
                SkyManager.Instance["NeoParacosm:DCSky"].Opacity = DCEffectOpacity;
            }

        }
    }
}
