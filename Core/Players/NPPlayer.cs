using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;

namespace NeoParacosm.Core.Players;

public class NPPlayer : ModPlayer
{
    int timer = 0;
    public bool NoMusic { get; set; } = false;

    public override void ResetEffects()
    {
        NoMusic = false;
    }

    public override void PostUpdate()
    {
        //Dust.QuickDust(new Point(Main.dungeonX, Main.dungeonY), Color.White);
        int researcherIndex = NPC.FindFirstNPC(NPCType<Researcher>());
        if (researcherIndex >= 0 && Main.npc[researcherIndex].Distance(Player.Center) > 500)
        {
            AscensionUISystem UISystem = GetInstance<AscensionUISystem>();
            UISystem.HideUI();
        }
        timer++;
    }
}
