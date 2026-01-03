using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Boss;

public class ResearcherBossUIState : UIState
{
    UITextPanel<string> textPanel;
    string textToDisplay = string.Empty;
    string displayedText = string.Empty;
    string[] textLines;
    public const int MAX_PART_COUNT = 7;
    public int PartCount { get; set; } = 0;

    public override void OnInitialize()
    {
        textPanel = new UITextPanel<string>(string.Empty, 1);
        textPanel.HAlign = 0.5f;
        textPanel.VAlign = 0.6f;
        textPanel.TextHAlign = 0f;
        Append(textPanel);
    }

    public override void OnActivate()
    {
        DisplayText();
    }

    void DisplayText()
    {
        textPanel.SetText("");
        textToDisplay = Language.GetTextValue($"Mods.NeoParacosm.NPCs.ResearcherBoss.Dialogue.P{PartCount}");
        textLines = textToDisplay.Split("\n");

        timer = 0;
        charIndex = 0;
        textLineIndex = 0;
        pauseDuration = 0;
        pauseTimer = 0;
        paused = false;
    }

    public override void OnDeactivate()
    {
        
    }

    int charIndex = 0;
    public const int DEFAULT_CHAR_INTERVAL = 3;
    public const int FAST_CHAR_INTERVAL = 1;
    public int charInterval = 3;
    int timer = 0;
    int textLineIndex = 0;
    int newLinePauseDuration = 60;
    int pauseDuration = 0;
    int pauseTimer = 0;
    bool paused = false;
    public override void Update(GameTime gameTime)
    {
        if (Main.gameInactive || Main.gamePaused)
        {
            return;
        }

        if (paused)
        {
            if (pauseTimer < pauseDuration)
            {
                pauseTimer++;
                return;
            }
            else
            {
                paused = false;
            }
        }

        string textLine = textLines[textLineIndex];
        if (timer % charInterval == 0)
        {
            if (charIndex < textLine.Length && textLine[charIndex] != '[')
            {
                SoundEngine.PlaySound(SoundID.Mech with { PitchRange = (0f, 0.2f), SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, MaxInstances = 10, Volume = 0.2f });
                displayedText = textLine[..(charIndex + 1)];
                //int duration = int.Parse(textLine.Substring(charIndex + 1, 2));
                charIndex++;

            }
            else if (charIndex < textLine.Length && textLine[charIndex] == '[')
            {
                int duration = int.Parse(textLine.Substring(charIndex + 1, 2));
                PauseText(duration);
                charIndex += 4;
            }
            else if (textLineIndex < textLines.Length - 1)
            {
                PauseText(newLinePauseDuration);
                textLineIndex++;
                charIndex = 0;
            }

            if (!paused && (textLineIndex >= textLines.Length - 1) && charIndex >= textLine.Length)
            {
                GetInstance<ResearcherBossUISystem>().HideUI();
            }
        }
        
        textPanel.SetText(displayedText);
        timer++;
    }

    void PauseText(int duration)
    {
        paused = true;
        pauseTimer = 0;
        pauseDuration = duration;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
    }
}
