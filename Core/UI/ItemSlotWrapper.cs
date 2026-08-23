using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace NeoParacosm.Core.UI;

// Copied from old ExampleMod
public class ItemSlotWrapper : UIElement
{
    public Item Item;
    private int _context;
    private float _scale;
    public Func<Item, bool> ValidItemFunc;

    public ItemSlotWrapper(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
    {
        _context = context;
        _scale = scale;
        Item = new Item();
        Item.SetDefaults(ItemID.None);

        Width.Set(TextureAssets.InventoryBack9.Width() * scale, 0f);
        Height.Set(TextureAssets.InventoryBack9.Height() * scale, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }

    protected override void DrawChildren(SpriteBatch spriteBatch)
    {
        base.DrawChildren(spriteBatch);
    }

    public static void DrawItemSlotCustom(SpriteBatch spriteBatch, ref Item item, int context, Vector2 position, Color lightColor = default)
    {
        if (lightColor == Color.Transparent)
            lightColor = Color.White;

        float inventoryScale = Main.inventoryScale;

        // Select texture and color based on context
        Texture2D texture2D = TextureAssets.InventoryBack.Value;
        Color color2 = Color.White;

        switch (context)
        {
            case 3:
                texture2D = TextureAssets.InventoryBack5.Value;
                break;
            case 4:
            case 32:
                texture2D = TextureAssets.InventoryBack2.Value;
                break;
            case 5:
            case 7:
                texture2D = TextureAssets.InventoryBack4.Value;
                break;
            case 6:
                texture2D = TextureAssets.InventoryBack7.Value;
                break;
            case 13:
                texture2D = TextureAssets.InventoryBack14.Value;
                color2 = new Color(200, 200, 200, 200);
                break;
            case 15:
                texture2D = TextureAssets.InventoryBack6.Value;
                break;
            case 28:
                texture2D = TextureAssets.InventoryBack7.Value;
                color2 = Color.White;
                break;
            case 29:
                color2 = new Color(53, 69, 127, 255);
                texture2D = TextureAssets.InventoryBack18.Value;
                break;
            default:
                texture2D = TextureAssets.InventoryBack9.Value;
                break;
        }

        // Draw background
        spriteBatch.Draw(texture2D, position, null, color2, 0f, Vector2.Zero, inventoryScale, SpriteEffects.None, 0f);

        // Draw item if present
        if (item.type > ItemID.None && item.stack > 0)
        {
            Vector2 vector = texture2D.Size() * inventoryScale;
            ItemSlot.DrawItemIcon(item, context, spriteBatch, position + vector / 2f, inventoryScale, 32f, lightColor);

            // Draw stack count
            if (item.stack > 1)
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(),
                    position + new Vector2(10f, 26f) * inventoryScale, lightColor, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
            }
        }
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
        DrawItemSlotCustom(spriteBatch, ref Item, _context, rectangle.TopLeft());
        Main.inventoryScale = oldScale;
    }
}
