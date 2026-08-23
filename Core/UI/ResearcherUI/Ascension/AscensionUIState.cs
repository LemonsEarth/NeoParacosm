using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Ascension;

public class AscensionUIState : UIState
{
    UIPanel MainPanel;
    InItemPanel InItemPanel;
    OutItemPanel OutItemPanel;
    UIImageButton CloseButton;
    UIImageButton ArrowButton;

    Color panelBG1 = Color.DarkRed;
    Color panelBG2 = Color.DarkSlateBlue;

    Color panelBorder1 = Color.Gold;
    Color panelBorder2 = Color.Lime;

    public override void OnInitialize()
    {
        MainPanel = new UIPanel();
        MainPanel.Width.Set(0, 0.2f);
        MainPanel.Height.Set(0, 0.15f);
        MainPanel.HAlign = 0.5f;
        MainPanel.VAlign = 0.4f;
        MainPanel.BackgroundColor = panelBG1;
        MainPanel.BorderColor = panelBorder1;
        Append(MainPanel);

        InItemPanel = new InItemPanel();
        InItemPanel.Width.Set(52, 0f);
        InItemPanel.Height.Set(52, 0f);
        InItemPanel.HAlign = 0.2f;
        InItemPanel.VAlign = 0.5f;
        MainPanel.Append(InItemPanel);

        UIText InText = new UIText("In");
        InText.HAlign = 0.235f;
        InText.VAlign = 0.9f;
        MainPanel.Append(InText);


        OutItemPanel = new OutItemPanel();
        OutItemPanel.Width.Set(52, 0f);
        OutItemPanel.Height.Set(52, 0f);
        OutItemPanel.HAlign = 0.8f;
        OutItemPanel.VAlign = 0.5f;
        MainPanel.Append(OutItemPanel);

        UIText OutText = new UIText("Out");
        OutText.HAlign = 0.78f;
        OutText.VAlign = 0.9f;
        MainPanel.Append(OutText);


        UIText infoText = new UIText("Place an infected item to ascend");
        infoText.HAlign = 0.5f;
        infoText.VAlign = 0.05f;
        MainPanel.Append(infoText);

        ArrowButton = new UIImageButton(ParacosmTextures.UIArrow);
        ArrowButton.Width.Set(32, 0f);
        ArrowButton.Height.Set(32, 0f);
        ArrowButton.HAlign = 0.5f;
        ArrowButton.VAlign = 0.5f;
        ArrowButton.OnLeftClick += OnArrowButtonClick;
        MainPanel.Append(ArrowButton);

        CloseButton = new UIImageButton(Request<Texture2D>("NeoParacosm/Common/Assets/Textures/UI/CloseButton"));
        CloseButton.Width.Set(32, 0f);
        CloseButton.Height.Set(32, 0f);
        CloseButton.HAlign = 1f;
        CloseButton.VAlign = 0f;
        CloseButton.OnLeftClick += OnCloseButtonClick;
        MainPanel.Append(CloseButton);
    }

    private void OnArrowButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        Item inItem = InItemPanel.Item;
        Item outItem = OutItemPanel.Item;
        if (!inItem.IsAir && outItem.IsAir)
        {
            if (Researcher.AscendableItems2.TryGetValue(inItem.type, out int value)
                && ResearcherQuest.Progress >= ResearcherQuest.ProgressState.CollectedData2)
            {
                OutItemPanel.Item = new Item(value);
            }
            else
            {
                OutItemPanel.Item = new Item(Researcher.AscendableItems[InItemPanel.Item.type]);
            }
            InItemPanel.Item.TurnToAir();
            SoundEngine.PlaySound(SoundID.Chat with { Pitch = 1f });
            SoundEngine.PlaySound(SoundID.Item29);
            if (ResearcherQuest.Progress == ResearcherQuest.ProgressState.TalkedAfterCollectingData)
            {
                ResearcherQuest.Progress = ResearcherQuest.ProgressState.AscendedItem;
            }
        }
    }

    private void OnCloseButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        GetInstance<AscensionUISystem>().HideUI();
    }

    public override void Update(GameTime gameTime)
    {
        float lerpT = (MathF.Sin(Main.GlobalTimeWrappedHourly / 3f) + 1) * 0.5f;
        MainPanel.BackgroundColor = Color.Lerp(panelBG1, panelBG2, lerpT);
        MainPanel.BorderColor = Color.Lerp(panelBorder1, panelBorder2, lerpT);
        MainPanel.Recalculate();
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
