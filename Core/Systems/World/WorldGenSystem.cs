using Microsoft.Xna.Framework.Input;
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
            WorldGen.Convert((int)(Main.LocalPlayer.position.X / 16), (int)(Main.LocalPlayer.position.Y / 16), BiomeConversionID.Crimson, 12, truebbbbb);
            //WorldUtils.Gen(new Point((int)(Main.LocalPlayer.position.X / 16), (int)(Main.LocalPlayer.position.Y / 16)), new Shapes.Circle(10, 5), new Actions.SetTile(TileID.Dirt));
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
