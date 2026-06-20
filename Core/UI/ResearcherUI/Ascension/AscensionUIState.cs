using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
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
    public override void OnInitialize()
    {
        MainPanel = new UIPanel();
        MainPanel.Width.Set(0, 0.2f);
        MainPanel.Height.Set(0, 0.1f);
        MainPanel.HAlign = 0.5f;
        MainPanel.VAlign = 0.4f;
        Append(MainPanel);

        InItemPanel = new InItemPanel();
        InItemPanel.Width.Set(64, 0f);
        InItemPanel.Height.Set(64, 0f);
        InItemPanel.HAlign = 0.2f;
        InItemPanel.VAlign = 1f;

        UIText InText = new UIText("In");
        InText.HAlign = 0.5f;
        InText.VAlign = 0.5f;
        InItemPanel.Append(InText);

        MainPanel.Append(InItemPanel);

        OutItemPanel = new OutItemPanel();
        OutItemPanel.Width.Set(64, 0f);
        OutItemPanel.Height.Set(64, 0f);
        OutItemPanel.HAlign = 0.8f;
        OutItemPanel.VAlign = 1f;

        UIText OutText = new UIText("Out");
        OutText.HAlign = 0.5f;
        OutText.VAlign = 0.5f;
        OutItemPanel.Append(OutText);

        MainPanel.Append(OutItemPanel);

        UIText infoText = new UIText("Place an infected item to ascend");
        infoText.HAlign = 0.5f;
        infoText.VAlign = 0.1f;
        MainPanel.Append(infoText);

        CloseButton = new UIImageButton(Request<Texture2D>("NeoParacosm/Common/Assets/Textures/UI/CloseButton"));
        CloseButton.Width.Set(32, 0f);
        CloseButton.Height.Set(32, 0f);
        CloseButton.HAlign = 0.5f;
        CloseButton.VAlign = 0.9f;
        CloseButton.OnLeftClick += OnButtonClick;
        MainPanel.Append(CloseButton);
    }

    private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        GetInstance<AscensionUISystem>().HideUI();
    }

    public override void Update(GameTime gameTime)
    {
        Item inItem = InItemPanel.itemSlot.Item;
        Item outItem = OutItemPanel.itemSlot.Item;
        if (!inItem.IsAir && outItem.IsAir)
        {
            if (Researcher.AscendableItems2.TryGetValue(inItem.type, out int value)
                && ResearcherQuest.Progress >= ResearcherQuest.ProgressState.CollectedData2)
            {
                OutItemPanel.itemSlot.Item = new Item(value);
            }
            else
            {
                OutItemPanel.itemSlot.Item = new Item(Researcher.AscendableItems[InItemPanel.itemSlot.Item.type]);
            }
            InItemPanel.itemSlot.Item.TurnToAir();
            SoundEngine.PlaySound(SoundID.Chat with { Pitch = 1f });
            SoundEngine.PlaySound(SoundID.Item29);
            if (ResearcherQuest.Progress == ResearcherQuest.ProgressState.TalkedAfterCollectingData)
            {
                ResearcherQuest.Progress = ResearcherQuest.ProgressState.AscendedItem;
            }
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
