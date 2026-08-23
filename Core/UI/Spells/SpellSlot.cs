using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems.Data;
using Terraria.DataStructures;
using Terraria.UI;

namespace NeoParacosm.Core.UI.Spells;

public class SpellSlot : ItemSlotWrapper
{
    public int slotID;
    static Asset<Texture2D> spellIconTexture;
    int context = 3;
    float scale = 1f;

    public SpellSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f) : base(context, scale)
    {
        this.context = context;
        this.scale = scale;
    }

    public override void OnInitialize()
    {
        spellIconTexture = Request<Texture2D>("NeoParacosm/Core/UI/Spells/SpellIcon");
        ValidItemFunc = item => item.IsAir || item.ModItem is BaseSpell;
    }

    public void SetItem(Item item)
    {
        Item = item;
        Main.LocalPlayer.NPCatalystPlayer().SetSpell(slotID, Item.ModItem as BaseSpell);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (Item == null || Item.IsAir)
        {
            //Main.NewText(Main.inventoryScale);
            float scale = 0.6f * 1.2f;
            var dims = GetDimensions().ToRectangle();

            // Match ItemSlot.Draw behavior: center the texture in the slot
            Vector2 position = dims.TopLeft() + new Vector2(dims.Width, dims.Height) * 0.5f;
            Vector2 origin = spellIconTexture.Size() * 0.5f;

            spriteBatch.Draw(spellIconTexture.Value, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }

    protected override void DrawChildren(SpriteBatch spriteBatch)
    {
        base.DrawChildren(spriteBatch);
    }

    public override void OnDeactivate()
    {
        /*if (itemSlot.Item.IsAir) return;

        Main.LocalPlayer.QuickSpawnItem(new EntitySource_DropAsItem(itemSlot.Item), itemSlot.Item, itemSlot.Item.stack);
        itemSlot.Item.TurnToAir();*/
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        Main.LocalPlayer.NPCatalystPlayer().SetSpell(slotID, Item.ModItem as BaseSpell);

        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
