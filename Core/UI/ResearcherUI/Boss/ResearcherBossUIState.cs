using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Misc;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Boss;

public class ResearcherBossUIState : UIState
{
    UITextPanel<string> textPanel;
    string textToDisplay = string.Empty;
    string displayedText = string.Empty;
    string[] textLines;
    public static int PartCount { get; private set; } = 0;

    public override void OnInitialize()
    {
        textPanel = new UITextPanel<string>(string.Empty, 1);
        textPanel.HAlign = 0.5f;
        textPanel.VAlign = 0.5f;
        textPanel.TextHAlign = 0f;
        Append(textPanel);
    }

    public override void OnActivate()
    {
        //ChatHelper.DisplayMessageOnClient(NetworkText.FromKey("Mods.NeoParacosm.NPCs.Researcher.NoteCloseMessage"), Color.Orange, Main.myPlayer);
        NextPart();
    }

    int maxPartCount = 7;
    void NextPart()
    {
        textPanel.SetText("");
        textToDisplay = Language.GetTextValue($"Mods.NeoParacosm.NPCs.ResearcherBoss.Dialogue.P{PartCount}");
        textLines = textToDisplay.Split("\n");
        PartCount++;
        if (PartCount >= maxPartCount)
        {
            PartCount = 0;
        }

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
    int charInterval = 3;
    int timer = 0;
    int textLineIndex = 0;
    int newLinePauseDuration = 60;
    int pauseDuration = 0;
    int pauseTimer = 0;
    bool paused = false;
    public override void Update(GameTime gameTime)
    {
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
