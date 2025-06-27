using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI;

public class ResearcherUIState : UIState
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

        CloseButton = new UIImageButton(ModContent.Request<Texture2D>("NeoParacosm/Core/UI/ResearcherUI/CloseButton"));
        CloseButton.Width.Set(32, 0f);
        CloseButton.Height.Set(32, 0f);
        CloseButton.VAlign = 0.5f;
        CloseButton.HAlign = 0.5f;
        CloseButton.OnLeftClick += OnButtonClick;
        MainPanel.Append(CloseButton);
    }

    private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        ModContent.GetInstance<ResearcherUISystem>().HideUI();
    }

    public override void Update(GameTime gameTime)
    {
        if (!InItemPanel.itemSlot.Item.IsAir && OutItemPanel.itemSlot.Item.IsAir)
        {
            OutItemPanel.itemSlot.Item = new Item(Researcher.AscendableItems[InItemPanel.itemSlot.Item.type]);
            InItemPanel.itemSlot.Item.TurnToAir();
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
