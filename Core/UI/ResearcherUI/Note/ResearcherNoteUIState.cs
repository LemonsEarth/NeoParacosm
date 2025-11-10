using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Note;

public class ResearcherNoteUIState : UIState
{
    UIImage MainImage;
    UIText MessageText;
    static Asset<Texture2D> noteTexture;

    public override void OnInitialize()
    {
        noteTexture = Request<Texture2D>("NeoParacosm/Core/UI/ResearcherUI/Note/ResearcherNoteUI");
        MainImage = new UIImage(noteTexture);
        MainImage.ScaleToFit = true;
        MainImage.HAlign = 0.5f;
        MainImage.VAlign = 0.5f;
        MainImage.Height.Set(0, 0); // set in update
        MainImage.Width.Set(0, 0);
        MainImage.SetPadding(24);
        MainImage.OnLeftDoubleClick += OnClick;
        Append(MainImage);

        MessageText = new UIText(string.Empty, 0.5f);
        MessageText.Width.Set(0, 0.9f);
        MessageText.Height.Set(0, 0.9f);
        MessageText.SetText(string.Empty, 0.45f, true);
        MessageText.HAlign = 0.5f;
        MessageText.VAlign = 0.5f;
        MessageText.IsWrapped = true;
        MessageText.TextOriginX = 0;
        MainImage.Append(MessageText);
    }


    private void OnClick(UIMouseEvent evt, UIElement listeningElement)
    {
        GetInstance<ResearcherNoteUISystem>().HideUI();
    }


    public override void OnActivate()
    {
        brightness = 1f;
        ChatHelper.DisplayMessageOnClient(NetworkText.FromLiteral("Message opened. Double click to close."), Color.Orange, Main.myPlayer);
    }

    public override void OnDeactivate()
    {

    }

    float brightness = 1f;

    public override void Update(GameTime gameTime)
    {
        MainImage.Height.Set(noteTexture.Width() * 1f, 0);
        MainImage.Width.Set(noteTexture.Width() * 1f, 0);
        MessageText.SetText(Language.GetTextValue($"Mods.NeoParacosm.NPCs.Researcher.ResearcherNoteMessage"), 0.45f, true);
        MainImage.Color = new Color(brightness, brightness, brightness);
        brightness = MathHelper.Lerp(brightness, 0.75f, 1 / 20f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (MainImage.ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
