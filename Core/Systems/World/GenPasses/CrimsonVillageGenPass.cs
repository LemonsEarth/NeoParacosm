using NeoParacosm.Content.Items.Weapons.Melee;
using StructureHelper.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;
using static Terraria.WorldGen;

namespace NeoParacosm.Core.Systems.World.GenPasses
{
    public class CrimsonVillageGenPass : GenPass
    {
        public CrimsonVillageGenPass(string name, float loadWeight) : base(name, loadWeight) { }

        readonly string CrimsonVillagePath = "Common/Assets/Structures/CrimsonVillageHouses";

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            if (!WorldGen.crimson)
            {
                return;
            }

            bool IsCrimsonTile(int tileType)
            {
                return tileType == TileID.Crimstone || tileType == TileID.CrimsonGrass || tileType == TileID.Crimsand;
            }

            for (int rep = 0; rep < LemonUtils.GetWorldSize(); rep++)
            {
                int startX = 0;
                int endX = Main.maxTilesX;
                int startY = 100;
                int endY = (int)GenVars.worldSurfaceHigh;
                for (int i = 0; i < MultiStructureGenerator.GetStructureCount(CrimsonVillagePath, NeoParacosm.Instance); i++)
                {
                    int attemptCounter = 0;
                    Point16 structureDims = MultiStructureGenerator.GetStructureDimensions(CrimsonVillagePath, NeoParacosm.Instance, i);

                    while (attemptCounter < 100000)
                    {
                        int x = WorldGen.genRand.Next(startX, endX);
                        int y = WorldGen.genRand.Next(startY, endY);
                        Tile tile = Main.tile[x, y];
                        Tile tileAbove = Main.tile[x, y - 1];
                        if (tile.HasTile && IsCrimsonTile(tile.TileType) && !tileAbove.HasTile)
                        {
                            startX = Math.Clamp(x - 150, 0, Main.maxTilesX);
                            endX = Math.Clamp(x + 150, 0, Main.maxTilesX);

                            startY = Math.Clamp(y - 150, 0, Main.maxTilesY);
                            endY = Math.Clamp(y + 150, 0, Main.maxTilesY);
                            Rectangle structureRect = new Rectangle(x, y, structureDims.X, structureDims.Y);

                            if (!GenVars.structures.CanPlace(structureRect, 5))
                            {
                                attemptCounter++;
                                continue;
                            }

                            // The code below generates mounds of dirt, crimson sand etc.
                            // Crimson grass looks ugly af when its not an edge tile,
                            // so the "else" part exists solely to generate normal dirt in the inner part of the mount
                            Point extraDirtPoint = new Point(x + structureDims.X / 2, y);
                            if (tile.TileType != TileID.CrimsonGrass)
                            {
                                WorldUtils.Gen(
                                    extraDirtPoint,
                                    new Shapes.Circle(Main.rand.Next(structureDims.X / 2 + 2, structureDims.X / 2 + 5), Main.rand.Next(3, 6)),
                                    new Actions.SetTile(tile.TileType));
                            }
                            else
                            {
                                int randWidth = Main.rand.Next(structureDims.X / 2 + 2, structureDims.X / 2 + 5);
                                int randHeight = Main.rand.Next(3, 6);
                                WorldUtils.Gen(
                                    extraDirtPoint,
                                    new Shapes.Circle(randWidth, randHeight),
                                    new Actions.SetTile(TileID.CrimsonGrass));
                                WorldUtils.Gen(
                                    extraDirtPoint + new Point(0, 1),
                                    new Shapes.Circle(randWidth, randHeight),
                                    new Actions.SetTile(TileID.Dirt));
                            }

                            // Actual structure gen point
                            Point16 adjustedPoint = new Point16(x, y - structureDims.Y);
                            // Placing the structure
                            MultiStructureGenerator.GenerateMultistructureSpecific(CrimsonVillagePath, i, adjustedPoint, NeoParacosm.Instance);
                            //Mod.Logger.Debug($"Generated Crimson House with index [{i}] at coordinates [{x}, {y}]");
                            GenVars.structures.AddProtectedStructure(structureRect, 3);
                            break;
                        }


                        attemptCounter++;
                        continue;
                    }
                }
            }

            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null) continue;
                int x = chest.x;
                int y = chest.y;

                Tile chestTile = Main.tile[x, y];
                if (chestTile.TileType != TileID.Containers || chestTile.TileFrameX != 14 * 36)
                {
                    continue;
                }

                for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (WorldGen.genRand.NextBool(10))
                        {
                            chest.item[inventoryIndex].SetDefaults(ItemType<ChainsawGun>());
                        }
                        break;
                    }
                }
            }
        }
    }
}
