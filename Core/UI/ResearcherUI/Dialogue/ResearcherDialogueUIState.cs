using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Dialogue;

public class ResearcherDialogueUIState : UIState
{
    UIPanel MainPanel;
    UIImageButton CloseButton;
    UIText Text;
    public override void OnInitialize()
    {
        MainPanel = new UIPanel();
        MainPanel.Width.Set(0, 0.5f);
        MainPanel.Height.Set(0, 0.35f);
        MainPanel.HAlign = 0.1f;
        MainPanel.VAlign = 0.9f;
        Append(MainPanel);

        Text = new UIText("jj", 2);
        Text.Width.Set(0, 0.8f);
        Text.Height.Set(0, 0.8f);
        Text.HAlign = 0.1f;
        Text.VAlign = 0.25f;
        Text.IsWrapped = true;
        MainPanel.Append(Text);

        CloseButton = new UIImageButton(ModContent.Request<Texture2D>("NeoParacosm/Common/Assets/Textures/UI/CloseButton"));
        CloseButton.Width.Set(32, 0f);
        CloseButton.Height.Set(32, 0f);
        CloseButton.HAlign = 0.9f;
        CloseButton.VAlign = 0.1f;
        CloseButton.OnLeftClick += OnButtonClick;
        MainPanel.Append(CloseButton);
    }

    private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        
        ModContent.GetInstance<ResearcherDialogueUISystem>().HideUI();
    }

    public override void OnActivate()
    {
        timer = 0;
        charIndex = 0;
        Text.SetText(string.Empty);
    }

    public override void OnDeactivate()
    {

    }

    int timer = 0;
    int charIndex = 0;
    int charInterval = 3;
    public override void Update(GameTime gameTime)
    {
        string text = "Reddit, am I the arsehole?\r\nMy mum (82F) told me (12M) to do the dishes (16) but I (12M) was too busy playing Fortnite (3 kills) so I (12M) grabbed my controller (DualShock 4) and threw it at her (138kph). She fucking died, and I (12M) went to prison (18 years). While in prison I (12M) incited several riots (3) and assumed leadership of a gang responsible for smuggling drugs (cocaine) into the country. I (12M) also ordered the assassination of several celebrities (Michael Jackson, Elvis Presley and Jeffrey Epstein) and planned a terrorist attack (9/11). Reddit, AITA?";
        float textHeight = Text.GetDimensions().Height;
        Text.SetText(text[..charIndex], 0.5f, true);
        if (timer % charInterval == 0 && charIndex < text.Length)
        {
            SoundEngine.PlaySound(SoundID.PlayerHit with { SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, MaxInstances = 1});
            charIndex++;
        }
        timer++;
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
