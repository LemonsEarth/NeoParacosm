using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Collections.Generic;
using Terraria.UI;

namespace NeoParacosm.Core.UI.Expeditions;

public class ExpeditionUISystem : ModSystem
{
    internal UserInterface userInterface;
    internal ExpeditionUIState UI;

    GameTime lastUpdateGameTime;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            userInterface = new UserInterface();
            UI = new ExpeditionUIState();
        }
    }

    public override void SetStaticDefaults()
    {
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
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Wire Selection"));
        if (mouseTextIndex != -1)
        {
            LegacyGameInterfaceLayer UILayer = new LegacyGameInterfaceLayer(
                "NeoParacosm:ExpeditionUI",
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
        //UI.OnInitialize();
    }

    internal void HideUI()
    {
        userInterface?.SetState(null);
    }
}
