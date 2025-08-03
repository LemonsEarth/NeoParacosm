using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Biomes.TheDepths;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Players;

public class NPPlayer : ModPlayer
{
    int timer = 0;

    float desaturateEffectOpacity = 0f;
    float desaturateEffectOpacityTimer = 0f;
    float maxDesaturateValue = 0.6f;
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
        if (NPC.FindFirstNPC(ModContent.NPCType<Researcher>()) > 0 && Main.npc[NPC.FindFirstNPC(ModContent.NPCType<Researcher>())].Distance(Player.Center) > 500)
        {
            AscensionUISystem UISystem = ModContent.GetInstance<AscensionUISystem>();
            UISystem.HideUI();
        }

        /*if (KeybindSystem.CrimsonSacrifice.JustReleased)
        {
            ResearcherUISystem UISystem = ModContent.GetInstance<ResearcherUISystem>();
            if (UISystem.userInterface.CurrentState == null)
            {
                UISystem.ShowUI();
            }
        }*/
        timer++;
    }

    public override void PostUpdateMiscEffects()
    {
        if (Player.InModBiome<DeadForestBiome>())
        {
            desaturateEffectOpacity = MathHelper.Lerp(0, maxDesaturateValue, desaturateEffectOpacityTimer / 60f);
            if (desaturateEffectOpacityTimer < 60) desaturateEffectOpacityTimer++;
            ScreenShaderData data = Filters.Scene.Activate("NeoParacosm:DesaturateShader").GetShader();
            data.UseProgress(desaturateEffectOpacity);
            
            Player.AddBuff(ModContent.BuffType<DeadForestDebuff>(), 2);
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
}
