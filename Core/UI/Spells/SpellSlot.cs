using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems.Data;
using Terraria.DataStructures;
using Terraria.UI;

namespace NeoParacosm.Core.UI.Spells;

public class SpellSlot : ItemSlotWrapper
{
    public ItemSlotWrapper itemSlot;
    public int slotID;
    static Asset<Texture2D> spellIconTexture;
    int context;

    static Item spellIcon;

    public SpellSlot(int context = ItemSlot.Context.ChestItem, float scale = 1f) : base(context, scale)
    {
        this.context = context;
    }

    public override void OnInitialize()
    {
        spellIcon = new Item(ItemType<SpellIcon>());
        spellIcon.SetDefaults(ItemType<SpellIcon>());
        spellIconTexture = Request<Texture2D>("NeoParacosm/Core/UI/Spells/SpellIcon");
        itemSlot = new ItemSlotWrapper(context, 1f)
        {
            ValidItemFunc = item => item.IsAir || item.ModItem is BaseSpell,
        };

        itemSlot.HAlign = 0.5f;
        itemSlot.VAlign = 0.5f;

        // Here we limit the items that can be placed in the slot. We are fine with placing an empty item in or a non-empty item that can be prefixed. Calling Prefix(-3) is the way to know if the item in question can take a prefix or not.
        Append(itemSlot);
    }

    public void SetItem(Item item)
    {
        itemSlot.Item = item;
        Main.LocalPlayer.NPCatalystPlayer().SetSpell(slotID, itemSlot.Item.ModItem as BaseSpell);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (itemSlot.Item == null || itemSlot.Item.IsAir)
        {
            //Main.NewText(Main.inventoryScale);
            float scale = 0.6f * 1.2f;
            var dims = itemSlot.GetDimensions().ToRectangle();

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
        Main.LocalPlayer.NPCatalystPlayer().SetSpell(slotID, itemSlot.Item.ModItem as BaseSpell);

        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
