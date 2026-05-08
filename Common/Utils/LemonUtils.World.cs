using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    public static int UnderworldDepth => Main.maxTilesY - 200;

    /// <summary>
    /// Returns 1 for Small Worlds, 2 for Medium Worlds, 3 for Large Worlds (and bigger?)
    /// </summary>
    /// <returns></returns>
    public static int GetWorldSize()
    {
        switch (Main.maxTilesX)
        {
            case >= 8400:
                return 3;
            case >= 6400:
                return 2;
            default:
                return 1;
        }
    }

    public static bool IsDungeonBrick(int tileID)
    {
        return tileID == TileID.BlueDungeonBrick || tileID == TileID.GreenDungeonBrick || tileID == TileID.PinkDungeonBrick;
    }

    /// <summary>
    /// Returns 1 for Classic and Journey, 2 for Expert, 3 for Master.
    /// Doubles value if For the Worthy seed is active.
    /// Useful for scaling values based on difficulty.
    /// </summary>
    /// <returns></returns>
    public static int GetDifficulty()
    {
        int difficulty = 1;
        if (Main.expertMode) difficulty++;
        if (Main.masterMode) difficulty++;
        if (Main.getGoodWorld) difficulty *= 2;
        return difficulty;
    }

    public static void GenerateStructureLoot(int structureX, int structureY, Point16 structureDims, List<List<(int, int)>> allPossibleItems)
    {
        for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
        {
            Chest chest = Main.chest[chestIndex];
            if (chest == null) continue;

            Rectangle structureRect = new Rectangle(structureX, structureY, structureDims.X, structureDims.Y);
            if (!structureRect.Contains(chest.x, chest.y))
            {
                continue;
            }

            for (int i = 0; i < allPossibleItems.Count; i++)
            {
                List<(int, int)> possibleItems = allPossibleItems[i];
                (int, int) chosenItem = Main.rand.NextFromCollection(possibleItems);
                chest.item[i].SetDefaults(chosenItem.Item1);
                chest.item[i].stack = chosenItem.Item2;
            }
        }
    }

    public static void GenerateStructureLoot(int structureX, int structureY, Point16 structureDims, List<List<(int, int)>> allPossibleItems, float randomizedStackOffsetMin, float randomizedStackOffsetMax)
    {
        for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
        {
            Chest chest = Main.chest[chestIndex];
            if (chest == null) continue;

            Rectangle structureRect = new Rectangle(structureX, structureY, structureDims.X, structureDims.Y);
            if (!structureRect.Contains(chest.x, chest.y))
            {
                continue;
            }

            for (int i = 0; i < allPossibleItems.Count; i++)
            {
                List<(int, int)> possibleItems = allPossibleItems[i];
                (int, int) chosenItem = Main.rand.NextFromCollection(possibleItems);
                chest.item[i].SetDefaults(chosenItem.Item1);
                if (chosenItem.Item2 > 1)
                {
                    chest.item[i].stack = Main.rand.Next((int)(chosenItem.Item2 * randomizedStackOffsetMin), (int)(chosenItem.Item2 * randomizedStackOffsetMax));
                }
                else
                {
                    chest.item[i].stack = chosenItem.Item2;
                }
            }
        }
    }

    /// <summary>
    /// Used for naturally generating an item in a vanilla chest. Use the wiki to find the appropriate chest's chestTileFrameX.
    /// If decreaseChanceDenominatorOnFail is true, the chance of the item generating will increase every unsuccessful attempt
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="chestTileFrameX"></param>
    /// <param name="chanceDenominator"></param>
    /// <param name="decreaseChanceDenominatorOnFail"></param>
    public static void GenerateItemInChest(int itemType, int chestTileFrameX, int chanceDenominator, bool decreaseChanceDenominatorOnFail = false)
    {
        int origChanceDenominator = chanceDenominator;
        for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
        {
            Chest chest = Main.chest[chestIndex];
            if (chest == null)
            {
                continue;
            }
            Tile chestTile = Main.tile[chest.x, chest.y];
            if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == chestTileFrameX * 36) // ivy chest
            {
                if (WorldGen.genRand.NextBool(chanceDenominator))
                {
                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemType);
                            break;
                        }
                    }
                }
                if (decreaseChanceDenominatorOnFail)
                {
                    if (chanceDenominator > 0)
                    {
                        chanceDenominator--;
                    }

                    if (chanceDenominator <= 0)
                    {
                        chanceDenominator = origChanceDenominator;
                    }
                }
            }
        }
    }

    public static float GetLightingAroundPos(Vector2 point, int tileRange)
    {
        float[,] lightingLevels = new float[tileRange * 2 + 1, tileRange * 2 + 1];
        int tileX = (int)(point.X / 16f);
        int tileY = (int)(point.Y / 16f);
        for (int row = -tileRange; row <= tileRange; row++)
        {
            for (int col = -tileRange; col <= tileRange; col++)
            {
                lightingLevels[row + tileRange, col + tileRange] = Lighting.Brightness(tileX + col, tileY + row);
            }
        }

        float sum = 0;
        foreach (var level in lightingLevels)
        {
            sum += level;
        }

        return sum;
    }

    public static bool IsTileTypeCrimson(int tileType)
    {
        return tileType == TileID.Crimstone || tileType == TileID.CrimsonGrass || tileType == TileID.Crimsand;
    }

    public static bool IsTileCrimson(Point point)
    {
        return Main.tile[point].HasTile && IsTileTypeCrimson(Main.tile[point].TileType);
    }

    public static bool IsTileTypeCorrupt(int tileType)
    {
        return tileType == TileID.Ebonstone || tileType == TileID.CorruptGrass || tileType == TileID.Ebonsand;
    }

    public static bool IsTileCorrupt(Point point)
    {
        return Main.tile[point].HasTile && IsTileTypeCorrupt(Main.tile[point].TileType);
    }

    public static bool IsTileDeadDirt(Point point)
    {
        return Main.tile[point].HasTile && Main.tile[point].TileType == TileType<DeadDirtBlock>();
    }

    public static bool IsTileTypeSnowy(int tileType)
    {
        return tileType == TileID.IceBlock || tileType == TileID.CorruptIce || tileType == TileID.FleshIce || tileType == TileID.SnowBlock;
    }

    public static bool IsTileSnowy(Point point)
    {
        return Main.tile[point].HasTile && IsTileTypeSnowy(Main.tile[point].TileType);
    }
}
