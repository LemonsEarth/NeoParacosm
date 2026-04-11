using NeoParacosm.Content.Items.Weapons.Magic;

namespace NeoParacosm.Core.Systems.World;

public class JungleStaffChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<JungleStaff>(), 10, 10, true);
    }
}