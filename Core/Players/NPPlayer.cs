using NeoParacosm.Content.Biomes.TheDepths;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.UI.ResearcherUI;
using Terraria.Graphics.Effects;

namespace NeoParacosm.Core.Players;

public class NPPlayer : ModPlayer
{
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
            ResearcherUISystem UISystem = ModContent.GetInstance<ResearcherUISystem>();
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
    }

    public override void PostUpdateMiscEffects()
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
}
