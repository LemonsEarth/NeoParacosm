using NeoParacosm.Content.Items.Accessories.Combat.Ranged;

namespace NeoParacosm.Core.Systems.World;

public class MagicMuzzleChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<MagicMuzzle>(), 2, 10, true);
    }
}