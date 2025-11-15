using NeoParacosm.Content.NPCs.Bosses.ResearcherBoss;
using System.Collections.Generic;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Boss;

public class ResearcherBossUISystem : ModSystem
{
    internal UserInterface userInterface;
    internal ResearcherBossUIState UI;

    GameTime lastUpdateGameTime;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            userInterface = new UserInterface();
            UI = new ResearcherBossUIState();
            UI.Activate();
        }
    }

    public override void Unload()
    {
        UI = null;
    }

    public override void UpdateUI(GameTime gameTime)
    {
        lastUpdateGameTime = gameTime;
        if (userInterface?.CurrentState != null)
        {
            userInterface.Update(gameTime);
            if (!NPC.AnyNPCs(NPCType<ResearcherBoss>()))
            {
                HideUI();
            }
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            LegacyGameInterfaceLayer UILayer = new LegacyGameInterfaceLayer(
                "NeoParacosm:ResearcherBossUI",
                () =>
                {
                    if (lastUpdateGameTime != null && userInterface?.CurrentState != null)
                    {
                        userInterface.Draw(Main.spriteBatch, lastUpdateGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI);
            layers.Insert(mouseTextIndex, UILayer);
        }
    }

    public override void ClearWorld()
    {
        HideUI();
    }

    public bool IsActive()
    {
        return userInterface.CurrentState != null;
    }

    public void ShowUI(int part, bool fastCharInterval = false)
    {
        if (part > ResearcherBossUIState.MAX_PART_COUNT)
        {
            part = ResearcherBossUIState.MAX_PART_COUNT;
        }
        UI.PartCount = part;
        UI.charInterval = fastCharInterval ? ResearcherBossUIState.FAST_CHAR_INTERVAL : ResearcherBossUIState.DEFAULT_CHAR_INTERVAL;
        userInterface?.SetState(UI);
    }

    public void HideUI()
    {
        userInterface?.SetState(null);
    }
}
