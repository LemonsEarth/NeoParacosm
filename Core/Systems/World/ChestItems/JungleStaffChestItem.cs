using NeoParacosm.Content.Items.Accessories.Combat.Ranged;
using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Core.Systems.World.GenPasses;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World;

public class JungleStaffChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<JungleStaff>(), 10, 10, true);
    }
}