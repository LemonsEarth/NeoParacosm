using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace NeoParacosm.Core.UI.Expeditions;

public class ExpeditionButton : UIElement
{
    static Asset<Texture2D> ExpeditionButtonBG;
    public Asset<Texture2D> ExpeditionIcon;
    public LocalizedText ExpeditionName;
    public LocalizedText ExpeditionDesc;

    public UIText ExpeditionNameText;

    public ExpeditionButton(Asset<Texture2D> bossIcon, LocalizedText expeditionName, LocalizedText expeditionDesc) : base()
    {
        ExpeditionIcon = bossIcon;
        ExpeditionName = expeditionName;
        ExpeditionDesc = expeditionDesc;
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);

        Main.NewText(ExpeditionName);
    }

    public override void OnInitialize()
    {
        ExpeditionButtonBG = Request<Texture2D>("NeoParacosm/Core/UI/Expeditions/ExpeditionButtonBG");

        ExpeditionNameText = new UIText(ExpeditionName.Value, 1f, true);
        ExpeditionNameText.HAlign = 0.4f;
        ExpeditionNameText.VAlign = 0.5f;
        ExpeditionNameText.Width.Set(0, 1f);
        ExpeditionNameText.Height.Set(0, 1f);
        ExpeditionNameText.TextOriginX = 0.75f;
        ExpeditionNameText.TextOriginY = 0.5f;
        Append(ExpeditionNameText);
    }

    public override void OnActivate()
    {
        base.OnActivate();
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        // When you override UIElement methods, don't forget call the base method
        // This helps to keep the basic behavior of the UIElement
        base.LeftMouseDown(evt);
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
    }

    public override void Update(GameTime gameTime)
    {
        ExpeditionNameText._textScale = 0.5f;
        ExpeditionNameText._isLarge = true;
        ExpeditionNameText.HAlign = 1f;
        ExpeditionNameText.VAlign = 0.5f;
        ExpeditionNameText.TextOriginX = 0.75f;
        ExpeditionNameText.TextOriginY = 0.5f;
        ExpeditionNameText.Height.Set(0, 0.5f);
        ExpeditionNameText.Recalculate();
        base.Update(gameTime);
        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            ExpeditionButtonBG.Value,
            GetDimensions().ToRectangle(),
            Color.White
            );

        Vector2 iconDrawPos = GetDimensions().Position() + new Vector2(40, 40);

        spriteBatch.Draw(
            ExpeditionIcon.Value,
            iconDrawPos,
            null,
            Color.White,
            0f,
            ExpeditionIcon.Size() * 0.5f,
            1f,
            SpriteEffects.None,
            0f
            );
    }
}
