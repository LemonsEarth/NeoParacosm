using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace NeoParacosm.Core.UI.Expeditions;

public class ExpeditionUIState : UIState
{
    UIPanel MainPanel;
    ExpeditionButton ExpeditionButton;
    public override void OnInitialize()
    {
        MainPanel = new UIPanel();
        MainPanel.Width.Set(0, 0.6f);
        MainPanel.Height.Set(0, 0.8f);
        MainPanel.HAlign = 0.5f;
        MainPanel.VAlign = 0.5f;
        MainPanel.BorderColor = Color.DarkSlateBlue;
        MainPanel.BackgroundColor = Color.DarkBlue;
        Append(MainPanel);


        ExpeditionButton = new ExpeditionButton(
            Request<Texture2D>("NeoParacosm/Core/UI/Expeditions/TestExpeditionIcon"),
            new LocalizedText("hh", "Test Expedition"),
            new LocalizedText("hh", "Test Expedition Description")
        );
        ExpeditionButton.Width.Set(320, 0f);
        ExpeditionButton.Height.Set(80, 0f);
        ExpeditionButton.HAlign = 0.25f;
        ExpeditionButton.VAlign = 0.15f;
        Append(ExpeditionButton);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        Main.NewText("click");
    }

    public override void OnActivate()
    {
        base.OnActivate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (MainPanel.ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
