using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Collections.Generic;
using System.Xml.Linq;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using static NeoParacosm.Core.Systems.ResearcherQuest;

namespace NeoParacosm.Core.UI.ResearcherUI.Dialogue;

public class ResearcherDialogueUIState : UIState
{
    UIPanel MainPanel;
    UIText DialogueText;
    UIImageButton CloseButton;

    UIPanel NamePanel;
    UIText NameText;

    UIPanel TalkOptionsPanel;
    UIText TalkOption;
    UIText SpecialOption;
    UIText ExitOption;

    int talkAmount = 0; // Amount of times talk has been pressed
    string TextToDisplay = "";
    public static Dictionary<int, int> ProgressTextAmount { get; private set; } = new Dictionary<int, int>()
    {
        {0, 1},
        {1, 4},
        {2, 3},
        {3, 1},
        {4, 3},
        {5, 3},
        {6, 5},
    };

    public override void OnInitialize()
    {
        // Main Panel and Dialogue
        MainPanel = new UIPanel();
        MainPanel.Width.Set(0, 0.5f);
        MainPanel.Height.Set(0, 0.35f);
        MainPanel.HAlign = 0.1f;
        MainPanel.VAlign = 0.9f;
        MainPanel.SetPadding(24);
        Append(MainPanel);

        DialogueText = new UIText(string.Empty, 0.5f);
        DialogueText.Width.Set(0, 0.8f);
        DialogueText.Height.Set(0, 0.8f);
        DialogueText.HAlign = 0.1f;
        DialogueText.VAlign = 0.25f;
        DialogueText.IsWrapped = true;
        DialogueText.TextOriginX = 0;
        MainPanel.Append(DialogueText);


        // Name Panel and Name
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


        // Talk Options
        TalkOptionsPanel = new UIPanel();
        TalkOptionsPanel.Width.Set(0, 0.07f);
        TalkOptionsPanel.Height.Set(0, 0.20f);
        TalkOptionsPanel.HAlign = 0.6f;
        TalkOptionsPanel.VAlign = 0.75f;
        TalkOptionsPanel.SetPadding(24);
        Append(TalkOptionsPanel);

        TalkOption = new UIText("Talk", 0.5f, true);
        TalkOption.Width.Set(0, 0.75f);
        TalkOption.Height.Set(0, 0.2f);
        TalkOption.HAlign = 0.5f;
        TalkOption.VAlign = 0f;
        TalkOption.OnLeftClick += OnTalkButtonClick;
        TalkOptionsPanel.Append(TalkOption);

        SpecialOption = new UIText("Ascend", 0.5f, true);
        SpecialOption.Width.Set(0, 0.75f);
        SpecialOption.Height.Set(0, 0.2f);
        SpecialOption.HAlign = 0.5f;
        SpecialOption.VAlign = 0.33f;
        SpecialOption.OnLeftClick += OnSpecialButtonClick;
        TalkOptionsPanel.Append(SpecialOption);

        ExitOption = new UIText("Exit", 0.5f, true);
        ExitOption.Width.Set(0, 0.75f);
        ExitOption.Height.Set(0, 0.2f);
        ExitOption.HAlign = 0.5f;
        ExitOption.VAlign = 0.66f;
        ExitOption.OnLeftClick += OnExitButtonClick;
        TalkOptionsPanel.Append(ExitOption);
    }

    void ProgressDialogue()
    {
        if (QuestProgress == ProgressState.CollectedData && talkAmount == 3)
        {
            QuestProgress = ProgressState.TalkedAfterCollectingData;
            SoundEngine.PlaySound(SoundID.Chat with { Pitch = 1f });
            talkAmount = 0;
        }

        if (talkAmount >= ProgressTextAmount[(int)QuestProgress])
        {
            talkAmount = 0;
        }

        TextToDisplay = Language.GetTextValue($"Mods.NeoParacosm.NPCs.Researcher.TalkDialogue.Progress.P{(int)QuestProgress}.T{talkAmount}");
        TextToDisplay += "\n\n" + $"{talkAmount + 1}/{ProgressTextAmount[(int)QuestProgress]}"; // dialogue left
        talkAmount++;
    }

    void ResetVars()
    {
        timer = 0;
        charIndex = 0;
        DialogueText.SetText(string.Empty);
        NameText.SetText("Sav", CalculateTextSize() * 1.2f, true);
    }

    private void OnTalkButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        if (charIndex < TextToDisplay.Length)
        {
            charIndex = TextToDisplay.Length - 1; // Skip dialogue
        }
        else
        {
            ProgressDialogue();
            ResetVars();
        }
    }

    private void OnSpecialButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        if (QuestProgress < ProgressState.TalkedAfterCollectingData)
        {
            return;
        }
        AscensionUISystem AscensionUI = GetInstance<AscensionUISystem>();
        if (AscensionUI.userInterface.CurrentState == null)
        {
            AscensionUI.ShowUI();
        }
        else
        {
            AscensionUI.HideUI();
        }
        talkAmount = 0;
    }

    private void OnExitButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        GetInstance<ResearcherDialogueUISystem>().HideUI();
    }


    public override void OnActivate()
    {
        talkAmount = 0;
        ProgressDialogue();
        ResetVars();
    }

    public override void OnDeactivate()
    {
        talkAmount = 0;
    }

    int timer = 0;
    int charIndex = 0;
    int charInterval = 3;
    public override void Update(GameTime gameTime)
    {
        SpecialButtonDisplay();
        //Main.NewText(talkAmount);
        DialogueText.SetText(TextToDisplay[..charIndex], CalculateTextSize(), true);
        int trueCharInterval = Main.mouseLeft ? 1 : charInterval;
        if (timer % trueCharInterval == 0 && charIndex < TextToDisplay.Length)
        {
            SoundEngine.PlaySound(SoundID.Mech with { PitchRange = (0f, 0.2f), SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, MaxInstances = 10, Volume = 0.2f });

            if (TextToDisplay[charIndex] == '[') // Skipping colored text n shi
            {
                int distanceToSkip = 0;
                for (int skipIndex = 0; skipIndex < 100; skipIndex++)
                {
                    if (TextToDisplay[charIndex + skipIndex] == ']')
                    {
                        break;
                    }
                    distanceToSkip++;
                }
                charIndex += distanceToSkip + 1;
            }
            else
            {
                if (charIndex + 2 < TextToDisplay.Length)
                {
                    charIndex += 2;
                }
                else
                {
                    charIndex += 1;
                }
            }
        }
        timer++;
    }

    float baseTextSize = 0.5f;
    float baseScreenWidth = 1920;
    float textSizeConstant => baseTextSize / (baseScreenWidth * 0.8f); // // 0.8 is the width UIText ui element takes up
    float CalculateTextSize() => textSizeConstant * 0.8f * Main.screenWidth; // 0.8 is the width UIText ui element takes up

    void SpecialButtonDisplay()
    {
        if (QuestProgress < ProgressState.TalkedAfterCollectingData)
        {
            SpecialOption.SetText(string.Empty);
        }
        else
        {
            SpecialOption.SetText("[c/FFFF00:Ascend]");
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
