using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Core.Systems;
using Terraria.DataStructures;
using Terraria.UI;

namespace NeoParacosm.Core.UI.ResearcherUI.Ascension;

internal class InItemPanel : ItemSlotWrapper
{
    public ItemSlotWrapper itemSlot;

    public override void OnInitialize()
    {
        itemSlot = new ItemSlotWrapper(ItemSlot.Context.BankItem, 0.85f)
        {
            ValidItemFunc = item => item.IsAir
            || Researcher.AscendableItems.ContainsKey(item.type)
            || (ResearcherQuest.Progress >= ResearcherQuest.ProgressState.CollectedData2 && Researcher.AscendableItems2.ContainsKey(item.type))
        };

        itemSlot.HAlign = 0.5f;
        itemSlot.VAlign = 0.5f;

        // Here we limit the items that can be placed in the slot. We are fine with placing an empty item in or a non-empty item that can be prefixed. Calling Prefix(-3) is the way to know if the item in question can take a prefix or not.
        Append(itemSlot);
    }

    public override void OnDeactivate()
    {
        if (itemSlot.Item.IsAir) return;

        Main.LocalPlayer.QuickSpawnItem(new EntitySource_DropAsItem(itemSlot.Item), itemSlot.Item, itemSlot.Item.stack);
        itemSlot.Item.TurnToAir();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
