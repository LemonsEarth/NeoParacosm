using NeoParacosm.Core.Systems.World.GenPasses;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World;

public class WorldGenSystem : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        InsertAfterTask(tasks, "Tile Cleanup", new CrimsonGenpasses("Building Bloody Settlement", 100f));
        InsertAfterTask(tasks, "Tile Cleanup", new CorruptionGenpasses("Hiding from the plague", 100f));
        InsertAfterTask(tasks, "Planting Trees", new DeadForestGenPass("Spreading Death", 100f));
        InsertAfterTask(tasks, "Micro Biomes", new DragonRemainsGenPass("Shifting Earth due to Powerful Presence", 100f));

        //int waterPlantsStep = tasks.FindIndex(genpass => genpass.Name.Equals("Water Plants"));
        //tasks.Insert(waterPlantsStep + 1, new PassLegacy("The Depths", GenerateDepths));
    }

    public override void PostUpdateWorld()
    {
        /*if (Main.keyState.IsKeyDown(Keys.B) && !Main.oldKeyState.IsKeyDown(Keys.B))
        {
            int attemptCount = 0;
            int maxAttemptCount = 10000;
            int placedCrosses = 0;
            int maxPlacedCrosses = 100;
            List<int> placedCrossXPos = new List<int>();

            while (attemptCount < maxAttemptCount && placedCrosses < maxPlacedCrosses)
            {
                int DeadForestRadius = 200 * LemonUtils.GetWorldSize();
                int startXTile = (int)MathHelper.Clamp(Main.dungeonX - DeadForestRadius, 0, Main.maxTilesX);
                int maxXTile = (int)MathHelper.Clamp(Main.dungeonX + DeadForestRadius, 0, Main.maxTilesX);
                int startYTile = (int)MathHelper.Clamp(Main.dungeonY - 80, 0, Main.maxTilesY);
                int maxYTile = (int)MathHelper.Clamp(Main.dungeonY + 80, 0, Main.maxTilesY);

                Point point = new Point(Main.rand.Next(startXTile, maxXTile), Main.rand.Next(startYTile, maxYTile));

                Point pointBelow = new Point(point.X, point.Y + 1);
                Tile tile = Main.tile[point];
                //Main.NewText(tile.TileType);

                Tile tileBelow = Main.tile[pointBelow];

                if (!tile.HasTile && tileBelow.HasTile && tileBelow.TileType == TileType<DeadDirtBlock>())
                {
                    foreach (var placedCrossXCoord in placedCrossXPos)
                    {
                        if (MathF.Abs(placedCrossXCoord - point.X) <= 3)
                        {
                            attemptCount++;
                            continue;
                        }
                    }
                    WorldGen.PlaceTile(point.X, point.Y, TileType<HolyCross>());
                    placedCrosses++;
                    placedCrossXPos.Add(point.X);
                }

                attemptCount++;
            }
            Main.NewText(placedCrosses);
        }*/
    }

    void InsertAfterTask(List<GenPass> tasks, string genpassName, GenPass genpass)
    {
        genpass.Name = $"[c/FFFF00:NeoParacosm: {genpass.Name}]";
        int step = tasks.FindIndex(pass => pass.Name.Equals(genpassName));
        tasks.Insert(step + 1, genpass);
    }

    /*void GenerateVerticalOceanTunnels()
    {
        int distanceBetweenTunnels = 50;
        int tunnelRepDistance = 20 * LemonUtils.GetWorldSize();
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
                repFailChanceDenominator /= 2;
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
        for (int amountOfTunnels = 0; amountOfTunnels < 4 * LemonUtils.GetWorldSize(); amountOfTunnels++)
        {
            for (int tunnelRepCount = 0; tunnelRepCount < 16; tunnelRepCount++)
            {
                /*if (Main.rand.NextBool(repFailChanceDenominator))
                {
                    repFailChanceDenominator = 8;
                    continue;
                }
                repFailChanceDenominator /= 2;
                int xStartPos = Main.maxTilesX - 350;
                int tunnelXPos = xStartPos + tunnelRepCount * tunnelRepDistance;

                int yStartPos = (int)Main.worldSurface - 50; // Ocean level isn't stored anywhere, literally just a guess
                int tunnelYPos = yStartPos + amountOfTunnels * distanceBetweenTunnels + Main.rand.Next(-4, 5);

                int steps = Main.rand.Next(8, 12);
                int size = Main.rand.Next(8, 10);
                WorldGen.digTunnel(tunnelXPos, tunnelYPos, 3, 0, 10, 8, true);
            }
        }
    }*/
}
