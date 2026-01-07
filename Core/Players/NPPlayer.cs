using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;

namespace NeoParacosm.Core.Players;

public class NPPlayer : ModPlayer
{
    int timer = 0;
    public static float savedMusicVolume { get; set; } = -1f;

    public override void ResetEffects()
    {
        if (savedMusicVolume != -1f)
        {
            Main.musicVolume = savedMusicVolume;
        }
    }

    public override void PostUpdate()
    {
        //Dust.QuickDust(new Point(Main.dungeonX, Main.dungeonY), Color.White);
        if (NPC.FindFirstNPC(NPCType<Researcher>()) > 0 && Main.npc[NPC.FindFirstNPC(NPCType<Researcher>())].Distance(Player.Center) > 500)
        {
            AscensionUISystem UISystem = GetInstance<AscensionUISystem>();
            UISystem.HideUI();
        }
        timer++;
    }
}
