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
        InItemPanel.HAlign = 0.25f;
        InItemPanel.VAlign = 0.5f;

        UIText InText = new UIText("In");
        InText.HAlign = 0.5f;
        InText.VAlign = 0.5f;
        InItemPanel.Append(InText);

        MainPanel.Append(InItemPanel);

        OutItemPanel = new OutItemPanel();
        OutItemPanel.Width.Set(64, 0f);
        OutItemPanel.Height.Set(64, 0f);
        OutItemPanel.HAlign = 0.75f;
        OutItemPanel.VAlign = 0.5f;

        UIText OutText = new UIText("Out");
        OutText.HAlign = 0.5f;
        OutText.VAlign = 0.5f;
        OutItemPanel.Append(OutText);

        MainPanel.Append(OutItemPanel);

        CloseButton = new UIImageButton(Request<Texture2D>("NeoParacosm/Common/Assets/Textures/UI/CloseButton"));
        CloseButton.Width.Set(32, 0f);
        CloseButton.Height.Set(32, 0f);
        CloseButton.VAlign = 0.5f;
        CloseButton.HAlign = 0.5f;
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
