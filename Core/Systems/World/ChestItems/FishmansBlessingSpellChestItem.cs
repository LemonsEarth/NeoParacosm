using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;

namespace NeoParacosm.Core.Systems.World;

public class FishmansBlessingSpellChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<FishmanBlessingSpell>(), 17, 4, true);
    }
}