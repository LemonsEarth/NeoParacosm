using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;
using System.Xml.Linq;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Dialogue;

public class ResearcherDialogueUIState : UIState
{
    UIPanel MainPanel;
    UIText DialogueText;
    UIImageButton CloseButton;

    UIPanel NamePanel;
    UIText NameText;

    UIText TalkOption;

    int talkAmount = 0; // Amount of times talk has been pressed
    public static Dictionary<int, int> progressTextAmount { get; private set; } = new Dictionary<int, int>()
    {
        {0, 1},
        {1, 4},
        {2, 3},
        {3, 1},
        {4, 3},
        {5, 3},
    };

    public override void OnInitialize()
    {
        MainPanel = new UIPanel();
        MainPanel.Width.Set(0, 0.5f);
        MainPanel.Height.Set(0, 0.35f);
        MainPanel.HAlign = 0.1f;
        MainPanel.VAlign = 0.9f;
        MainPanel.SetPadding(24);
        Append(MainPanel);

        NamePanel = new UIPanel();
        NamePanel.Width.Set(0, 0.10f);
        NamePanel.Height.Set(0, 0.05f);
        NamePanel.HAlign = 0.06f;
        NamePanel.VAlign = 0.56f;
        Append(NamePanel);

        NameText = new UIText(string.Empty, 1);
        NameText.Width.Set(0, 0.9f);
        NameText.Height.Set(0, 0.8f);
        NameText.HAlign = 0.1f;
        NameText.VAlign = 0f;
        NameText.IsWrapped = true;
        NameText.TextOriginX = 0;
        NamePanel.Append(NameText);

        DialogueText = new UIText(string.Empty, 0.5f);
        DialogueText.Width.Set(0, 0.8f);
        DialogueText.Height.Set(0, 0.8f);
        DialogueText.HAlign = 0.1f;
        DialogueText.VAlign = 0.25f;
        DialogueText.IsWrapped = true;
        DialogueText.TextOriginX = 0;
        MainPanel.Append(DialogueText);

        CloseButton = new UIImageButton(ModContent.Request<Texture2D>("NeoParacosm/Common/Assets/Textures/UI/CloseButton"));
        CloseButton.Width.Set(32, 0f);
        CloseButton.Height.Set(32, 0f);
        CloseButton.HAlign = 0.9f;
        CloseButton.VAlign = 0.1f;
        CloseButton.OnLeftClick += OnCloseButtonClick;
        MainPanel.Append(CloseButton);

        TalkOption = new UIText(string.Empty, 0.5f, true);
        TalkOption.Width.Set(0, 0.05f);
        TalkOption.Height.Set(0, 0.05f);
        TalkOption.HAlign = 0.6f;
        TalkOption.HAlign = 0.4f;
        TalkOption.OnLeftClick += OnTalkButtonClick;
        Append(TalkOption);
    }

    private void OnTalkButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        
    }

    private void OnCloseButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        ModContent.GetInstance<ResearcherDialogueUISystem>().HideUI();
    }

    public override void OnActivate()
    {
        timer = 0;
        charIndex = 0;
        DialogueText.SetText(string.Empty);
        int researcherID = NPC.FindFirstNPC(ModContent.NPCType<Researcher>());
        if (researcherID > -1)
        {
            NameText.SetText(Main.npc[researcherID].FullName, CalculateTextSize(), true);
        }
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
        float textHeight = DialogueText.GetDimensions().Height;
        int textSize = Main.screenWidth;
        DialogueText.SetText(text[..charIndex], CalculateTextSize(), true);
        int trueCharInterval = Main.mouseLeft ? 1 : charInterval;
        if (timer % trueCharInterval == 0 && charIndex < text.Length)
        {
            SoundEngine.PlaySound(SoundID.PlayerHit with { SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, MaxInstances = 1 });
            charIndex++;
        }
        timer++;
    }

    const float baseTextSize = 0.5f;
    const float baseScreenWidth = 1920;
    const float textSizeConstant = baseTextSize / (baseScreenWidth * 0.8f); // // 0.8 is the width UIText ui element takes up
    float CalculateTextSize() => textSizeConstant * 0.8f * Main.screenWidth; // 0.8 is the width UIText ui element takes up

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (MainPanel.ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
