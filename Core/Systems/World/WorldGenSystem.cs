using Microsoft.Xna.Framework.Input;
using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Placeable.Tiles.Depths;
using NeoParacosm.Content.Items.Weapons.Melee;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World;

public class WorldGenSystem : ModSystem
{
    int WorldSize => LemonUtils.GetWorldSize();
    readonly string CrimsonVillagePath = "Common/Assets/Structures/CrimsonVillageHouses";
    readonly string DragonRemainsPath = "Common/Assets/Structures/DragonRemainsSanctuary";

    static int UnderworldDepth => Main.maxTilesY - 200;

    void GenerateCrimsonVillage(GenerationProgress progress, GameConfiguration config)
    {
        if (!WorldGen.crimson)
        {
            return;
        }

        bool IsCrimsonTile(int tileType)
        {
            return tileType == TileID.Crimstone || tileType == TileID.CrimsonGrass || tileType == TileID.Crimsand;
        }

        for (int rep = 0; rep < WorldSize; rep++)
        {
            int startX = 0;
            int endX = Main.maxTilesX;
            int startY = 0;
            int endY = (int)GenVars.worldSurfaceHigh;
            for (int i = 0; i < MultiStructureGenerator.GetStructureCount(CrimsonVillagePath, Mod); i++)
            {
                int attemptCounter = 0;
                Point16 structureDims = MultiStructureGenerator.GetStructureDimensions(CrimsonVillagePath, Mod, i);


                while (attemptCounter < 100000)
                {
                    int x = WorldGen.genRand.Next(startX, endX);
                    int y = WorldGen.genRand.Next(startY, endY);
                    Tile tile = Main.tile[x, y];
                    Tile tileAbove = Main.tile[x, Math.Clamp(y - 1, 0, Main.maxTilesY)];

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
                        Point16 point = new Point16(x, y);
                        MultiStructureGenerator.GenerateMultistructureSpecific(CrimsonVillagePath, i, point, Mod);
                        Mod.Logger.Debug($"Generated Crimson House with index [{i}] at coordinates [{x}, {y}]");
                        GenVars.structures.AddProtectedStructure(structureRect, 5);
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

    void GenerateDeadForest(GenerationProgress progress, GameConfiguration config)
    {
        int Radius = 200 * WorldSize;
        Point startPos = new Point(Main.dungeonX, Main.dungeonY + 30);
        //LemonUtils.DustCircle(startPos.ToWorldCoordinates(), 8, 8, DustID.Granite, 10);
        for (int i = -Radius; i < Radius; i++)
        {
            for (int j = -Radius; j < Radius; j++)
            {
                Point pos = startPos + new Point(i, j);
                pos = new Point(Math.Clamp(pos.X, 0, Main.maxTilesX), Math.Clamp(pos.Y, 0, Main.maxTilesY));
                if (startPos.ToWorldCoordinates().Distance(pos.ToWorldCoordinates()) > Radius * 16)
                {
                    continue;
                }
                Tile tile = Main.tile[pos];
                //Projectile.NewProjectile(new EntitySource_Misc("gewg"), pos.ToWorldCoordinates(), Vector2.Zero, ProjectileType<DragonRemainsPulseShield>(), 1, 1);
                if (tile.HasTile)
                {
                    switch (tile.TileType)
                    {
                        case TileID.Dirt or TileID.ClayBlock or TileID.Grass or TileID.Sand or TileID.CorruptGrass or TileID.CrimsonGrass or TileID.Ebonsand or TileID.Crimsand:
                            //WorldGen.ConvertTile(pos.X, pos.Y, TileType<DeadDirtBlock>());
                            WorldGen.ConvertTile(pos.X, pos.Y, TileType<DeadDirtBlock>());
                            break;
                    }
                }
            }
        }
    }

    void GenerateDragonRemains(GenerationProgress progress, GameConfiguration config)
    {
        int attemptCount = 0;
        int maxAttempts = 1000000;
        Point16 structureDims = Generator.GetStructureDimensions(DragonRemainsPath, Mod);
        while (attemptCount < maxAttempts)
        {
            bool conflict = false;
            int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.33f), (int)(Main.maxTilesX * 0.66f));
            int y = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, UnderworldDepth - 100 - structureDims.Y);
            Rectangle structureRect = new Rectangle(x, y, structureDims.X, structureDims.Y);
            for (int i = 0; i < structureDims.X; i++)
            {
                if (conflict) break;
                for (int j = 0; j < structureDims.Y; j++)
                {
                    Tile tile = Main.tile[x + i, y + j];
                    bool cantPlace = !GenVars.structures.CanPlace(structureRect, 0);
                    bool isSpecialBrick = IsDungeonBrick(tile.TileType) || tile.TileType == TileID.LihzahrdBrick;
                    if (cantPlace || (tile.HasTile && isSpecialBrick))
                    {
                        attemptCount++;
                        conflict = true;
                        break;
                    }
                }
            }
            if (!conflict)
            {
                GenVars.structures.AddProtectedStructure(structureRect, 0);
                Generator.GenerateStructure(DragonRemainsPath, new Point16(x, y), Mod);
                Mod.Logger.Info($"Generated Dragon Remains At ({x}, {y})");
                break;
            }
        }
    }

    void GenerateDepths(GenerationProgress progress, GameConfiguration config)
    {
        int startTileX = Main.maxTilesX - 350;
        int startTileY = (int)Main.worldSurface - 100;

        int endTileX = Main.maxTilesX;
        int endTileY = startTileY + 4 * WorldSize * 75;
        for (int yLevel = 0; yLevel < 4 * WorldSize; yLevel++)
        {
            for (int xPos = 0; xPos < 5; xPos++)
            {
                WorldGen.OreRunner(startTileX + xPos * 100, startTileY + yLevel * 100, 200, 10, (ushort)TileType<DepthStoneBlock>());
            }
        }
        GenerateVerticalOceanTunnels();
        GenerateHorizontalOceanTunnels();

        for (int i = startTileX; i < endTileX; i++)
        {
            for (int j = startTileY; j < endTileY; j++)
            {
                if (WorldGen.InWorld(i, j))
                {

                    WorldGen.PlaceWall(i, j, WallType<DepthStoneWall>());

                    if (Main.tile[i, j].HasTile)
                    {
                        if (Main.tile[i, j].TileType != (ushort)TileType<DepthStoneBlock>())
                        {
                            WorldGen.PlaceTile(i, j, (ushort)TileType<DepthStoneBlock>(), forced: true);
                        }

                    }
                    else
                    {
                        WorldGen.PlaceLiquid(i, j, (byte)LiquidID.Water, 255);
                    }

                }
            }
        }
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        InsertAfterTask(tasks, "Tile Cleanup", "Crimson Village", GenerateCrimsonVillage);
        InsertAfterTask(tasks, "Planting Trees", "Dead Forest", GenerateDeadForest);
        InsertAfterTask(tasks, "Micro Biomes", "Dragon Remains", GenerateDragonRemains);

        //int waterPlantsStep = tasks.FindIndex(genpass => genpass.Name.Equals("Water Plants"));
        //tasks.Insert(waterPlantsStep + 1, new PassLegacy("The Depths", GenerateDepths));
    }

    public override void PostUpdateWorld()
    {

        if (Main.keyState.IsKeyDown(Keys.B) && !Main.oldKeyState.IsKeyDown(Keys.B))
        {
            /*int Radius = 100 * worldSize;
            Point startPos = new Point(Main.maxTilesX / 2, (int)GenVars.worldSurfaceLow + 100);
            //LemonUtils.DustCircle(startPos.ToWorldCoordinates(), 8, 8, DustID.Granite, 10);
            for (int i = -Radius; i < Radius; i++)
            {
                for (int j = -Radius; j < Radius; j++)
                {
                    Point pos = startPos + new Point(i, j);
                    if (startPos.ToWorldCoordinates().Distance(pos.ToWorldCoordinates()) > Radius * 16)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[pos];
                    //Projectile.NewProjectile(new EntitySource_Misc("gewg"), pos.ToWorldCoordinates(), Vector2.Zero, ProjectileType<DragonRemainsPulseShield>(), 1, 1);
                    if (tile.HasTile)
                    {
                        switch (tile.TileType)
                        {
                            case TileID.Dirt or TileID.ClayBlock or TileID.Grass:
                                //WorldGen.ConvertTile(pos.X, pos.Y, TileType<DeadDirtBlock>());
                                WorldGen.ConvertTile(pos.X, pos.Y, TileType<DeadDirtBlock>());
                                break;
                        }

                    }
                }
            }*/
        }
    }

    void InsertAfterTask(List<GenPass> tasks, string genpassName, string newGenPassName, WorldGenLegacyMethod method)
    {
        int step = tasks.FindIndex(genpass => genpass.Name.Equals(genpassName));
        tasks.Insert(step + 1, new PassLegacy($"[c/AA00FF:NeoParacosm: {newGenPassName}]", method));
    }

    void GenerateVerticalOceanTunnels()
    {
        int distanceBetweenTunnels = 50;
        int tunnelRepDistance = 20 * WorldSize;
        //int repFailChanceDenominator = 8;
        for (int amountOfTunnels = 0; amountOfTunnels < 10; amountOfTunnels++)
        {
            for (int tunnelRepCount = 0; tunnelRepCount < 14; tunnelRepCount++)
            {
                /*if (Main.rand.NextBool(repFailChanceDenominator))
                {
                    repFailChanceDenominator = 16;
                    continue;
                }
                repFailChanceDenominator /= 2;*/
                int xStartPos = Main.maxTilesX - 300;
                int tunnelXPos = xStartPos + amountOfTunnels * distanceBetweenTunnels + Main.rand.Next(-4, 5);

                int yStartPos = (int)Main.worldSurface - 100; // Ocean level isn't stored anywhere, literally just a guess
                int tunnelYPos = yStartPos + tunnelRepCount * tunnelRepDistance;

                int steps = Main.rand.Next(8, 12);
                int size = Main.rand.Next(8, 10);
                WorldGen.digTunnel(tunnelXPos, tunnelYPos, 0, 3, 10, 8, true);
            }
        }
    }

    void GenerateHorizontalOceanTunnels()
    {
        int distanceBetweenTunnels = 75;
        int tunnelRepDistance = 20;
        //int repFailChanceDenominator = 4;
        for (int amountOfTunnels = 0; amountOfTunnels < 4 * WorldSize; amountOfTunnels++)
        {
            for (int tunnelRepCount = 0; tunnelRepCount < 16; tunnelRepCount++)
            {
                /*if (Main.rand.NextBool(repFailChanceDenominator))
                {
                    repFailChanceDenominator = 8;
                    continue;
                }
                repFailChanceDenominator /= 2;*/
                int xStartPos = Main.maxTilesX - 350;
                int tunnelXPos = xStartPos + tunnelRepCount * tunnelRepDistance;

                int yStartPos = (int)Main.worldSurface - 50; // Ocean level isn't stored anywhere, literally just a guess
                int tunnelYPos = yStartPos + amountOfTunnels * distanceBetweenTunnels + Main.rand.Next(-4, 5);

                int steps = Main.rand.Next(8, 12);
                int size = Main.rand.Next(8, 10);
                WorldGen.digTunnel(tunnelXPos, tunnelYPos, 3, 0, 10, 8, true);
            }
        }
    }

    public static bool IsDungeonBrick(int tileID)
    {
        return tileID == TileID.BlueDungeonBrick || tileID == TileID.GreenDungeonBrick || tileID == TileID.PinkDungeonBrick;
    }
}
