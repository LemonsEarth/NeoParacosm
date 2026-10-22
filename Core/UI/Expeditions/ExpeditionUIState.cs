using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoParacosm.Core.Systems.Expeditions;
using SubworldLibrary;
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
            new LocalizedText(LemonUtils.GetLocKey("Expeditions", "TestExpedition.DisplayName"), "hhDesc"),
            new LocalizedText(LemonUtils.GetLocKey("Expeditions", "TestExpedition.Description"), "hhDesc")
        );
        ExpeditionButton.Width.Set(320, 0f);
        ExpeditionButton.Height.Set(80, 0f);
        ExpeditionButton.HAlign = 0.25f;
        ExpeditionButton.VAlign = 0.15f;
        ExpeditionButton.OnLeftDoubleClick += ExpeditionButton_OnLeftDoubleClick;
        Append(ExpeditionButton);
    }

    private void ExpeditionButton_OnLeftDoubleClick(UIMouseEvent evt, UIElement listeningElement)
    {
        ExpeditionUISystem system = GetInstance<ExpeditionUISystem>();
        system.HideUI();
        SubworldSystem.Enter<TestExpedition>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
        {
            ExpeditionUISystem system = GetInstance<ExpeditionUISystem>();
            system.HideUI();
        }
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
