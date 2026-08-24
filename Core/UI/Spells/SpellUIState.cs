using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace NeoParacosm.Core.UI.Spells;

public class SpellUIState : UIState
{
    DraggableUIPanel MainPanel;
    UIText text;
    SpellSlot[] SpellSlots = new SpellSlot[3];
    NPCatalystPlayer Player => Main.LocalPlayer.NPCatalystPlayer();
    static Asset<Texture2D> panelBG;
    static Asset<Texture2D> panelBorder;
    public override void OnInitialize()
    {
        panelBG = Request<Texture2D>("NeoParacosm/Core/UI/Spells/SpellPanelBackground");
        panelBorder = Request<Texture2D>("NeoParacosm/Core/UI/Spells/SpellPanelBorder");
        MainPanel = new DraggableUIPanel(panelBG, panelBorder);
        MainPanel.Width.Set(0, 0.15f);
        MainPanel.Height.Set(0, 0.15f);
        MainPanel.HAlign = 0.05f;
        MainPanel.VAlign = 0.9f;
        MainPanel.BorderColor = Color.Yellow;
        MainPanel.BackgroundColor = Color.DarkSlateBlue;
        Append(MainPanel);

        text = new UIText("Spells", 0.6f, true);
        text.TextColor = Color.Yellow;
        text.Width.Set(0, 1f);
        text.Height.Set(0, 0.5f);
        text.HAlign = 0.5f;
        text.VAlign = 0.15f;
        MainPanel.Append(text);

        SpellSlots = new SpellSlot[Player.maxSpellSlots];
        for (int i = 0; i < Player.maxSpellSlots; i++)
        {
            SpellSlots[i] = new SpellSlot(ItemSlot.Context.InventoryItem);
            SpellSlots[i].slotID = i;
            SpellSlots[i].Initialize();
            if (Player.EquippedSpells[i] == null)
            {
                SpellSlots[i].SetItem(new Item(ItemID.None));
            }
            else
            {
                SpellSlots[i].SetItem(Player.EquippedSpells[i].Item);
            }
        }

        for (int i = 0; i < SpellSlots.Length; i++)
        {
            SpellSlots[i].Width.Set(52, 0f);
            SpellSlots[i].Height.Set(52, 0f);
            SpellSlots[i].HAlign = 0.33f + 0.33f * i;
            SpellSlots[i].VAlign = 0.7f;
            MainPanel.Append(SpellSlots[i]);
        }
    }

    public override void OnActivate()
    {

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        MainPanel.Width.Set(0, 0.15f * Main.UIScale);
        MainPanel.Height.Set(0, 0.15f * Main.UIScale);
        MainPanel.Recalculate();
        for (int i = 0; i < SpellSlots.Length; i++)
        {
            SpellSlots[i].HAlign = 0.25f + 0.25f * i;
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
