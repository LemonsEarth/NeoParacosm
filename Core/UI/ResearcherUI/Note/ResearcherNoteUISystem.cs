using System.Collections.Generic;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Note;

public class ResearcherNoteUISystem : ModSystem
{
    internal UserInterface userInterface;
    internal ResearcherNoteUIState UI;

    GameTime lastUpdateGameTime;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            userInterface = new UserInterface();
            UI = new ResearcherNoteUIState();
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
        if (userInterface?.CurrentState != null) userInterface.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            LegacyGameInterfaceLayer UILayer = new LegacyGameInterfaceLayer(
                "NeoParacosm:ResearcherNoteUI",
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

    internal void ShowUI()
    {
        userInterface?.SetState(UI);
    }

    internal void HideUI()
    {
        userInterface?.SetState(null);
    }
}
