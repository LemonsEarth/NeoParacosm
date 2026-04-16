using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;

namespace NeoParacosm.Core.Systems.World;

public class HailfireSpellChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<HailfireSpell>(), 11, 6, true);
    }
}