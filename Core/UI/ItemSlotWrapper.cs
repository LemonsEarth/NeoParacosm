﻿using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace NeoParacosm.Core.UI;

// Copied from old ExampleMod
internal class ItemSlotWrapper : UIElement
{
    internal Item Item;
    private readonly int _context;
    private readonly float _scale;
    internal Func<Item, bool> ValidItemFunc;

    public ItemSlotWrapper(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
    {
        _context = context;
        _scale = scale;
        Item = new Item();
        Item.SetDefaults(0);

        Width.Set(TextureAssets.InventoryBack9.Width() * scale, 0f);
        Height.Set(TextureAssets.InventoryBack9.Height() * scale, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;
        Rectangle rectangle = GetDimensions().ToRectangle();

        if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
            if (ValidItemFunc == null || ValidItemFunc(Main.mouseItem))
            {
                // Handle handles all the click and hover actions based on the context.
                ItemSlot.Handle(ref Item, _context);
            }
        }
        // Draw draws the slot itself and Item. Depending on context, the color will change, as will drawing other things like stack counts.
        ItemSlot.Draw(spriteBatch, ref Item, _context, rectangle.TopLeft());
        Main.inventoryScale = oldScale;
    }
}
