using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Ascension;

internal class OutItemPanel : ItemSlotWrapper
{
    int timer = 0;
    int index = 0;
    public override void OnInitialize()
    {
        ValidItemFunc = item => item.IsAir;
    }

    public override void OnDeactivate()
    {
        if (Item.IsAir) return;

        Main.LocalPlayer.QuickSpawnItem(new EntitySource_DropAsItem(Item), Item, Item.stack);
        Item.TurnToAir();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        int time = (int)(Main.GlobalTimeWrappedHourly * 100);
        if (time % 60 == 0)
        {
            index = (index + 1) % Researcher.AscendableItems.Count;
        }
        timer++;
        base.Draw(spriteBatch);
        if (Item == null || Item.IsAir)
        {
            //Main.NewText(Main.inventoryScale);
            float scale = 0.6f * 1.2f;
            var dims = GetDimensions().ToRectangle();

            // Match ItemSlot.Draw behavior: center the texture in the slot
            Vector2 position = dims.TopLeft() + new Vector2(dims.Width, dims.Height) * 0.5f;
            var ascendedItems = Researcher.AscendableItems.Values.ToList();
            Main.instance.LoadItem(ascendedItems[index]);
            Texture2D texture = TextureAssets.Item[ascendedItems[index]].Value;
            Vector2 origin = texture.Size() * 0.5f;

            spriteBatch.Draw(texture, position, null, Color.White * 0.5f, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
