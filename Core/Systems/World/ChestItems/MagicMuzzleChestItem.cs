using NeoParacosm.Content.Items.Accessories.Combat.Ranged;
using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Core.Systems.World.GenPasses;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World;

public class MagicMuzzleChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        int chance = 10;
        for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
        {
            Chest chest = Main.chest[chestIndex];
            if (chest == null)
            {
                continue;
            }
            Tile chestTile = Main.tile[chest.x, chest.y];
            if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 2 * 36) // locked gold chest
            {
                if (WorldGen.genRand.NextBool(chance))
                {
                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(ItemType<MagicMuzzle>());
                            break;
                        }
                    }
                }
                else
                {
                    if (chance > 0)
                    {
                        chance--;
                    }

                    if (chance <= 0)
                    {
                        chance = 10;
                    }
                }
            }
        }
    }
}