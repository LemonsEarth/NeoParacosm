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

public class WorldGenSystem : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        InsertAfterTask(tasks, "Tile Cleanup", "Crimson Village", new CrimsonVillageGenPass("Building Bloody Settlement", 100f));
        InsertAfterTask(tasks, "Planting Trees", "Dead Forest", new DeadForestGenPass("Spreading Death", 100f));
        InsertAfterTask(tasks, "Micro Biomes", "Dragon Remains", new DragonRemainsGenPass("Shifting Earth due to Powerful Presence", 100f));

        //int waterPlantsStep = tasks.FindIndex(genpass => genpass.Name.Equals("Water Plants"));
        //tasks.Insert(waterPlantsStep + 1, new PassLegacy("The Depths", GenerateDepths));
    }

    public override void PostUpdateWorld()
    {
        /*if (Main.keyState.IsKeyDown(Keys.B) && !Main.oldKeyState.IsKeyDown(Keys.B))
        {
            int Radius = 100 * worldSize;
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
            }
        }*/
    }

    void InsertAfterTask(List<GenPass> tasks, string genpassName, string newGenPassName, GenPass genpass)
    {
        int step = tasks.FindIndex(genpass => genpass.Name.Equals(genpassName));
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
