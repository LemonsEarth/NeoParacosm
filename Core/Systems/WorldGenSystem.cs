using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework.Input;

namespace NeoParacosm.Core.Systems;

public class WorldGenSystem : ModSystem
{
    readonly string CrimsonVillagePath = "Common/Assets/Structures/CrimsonVillageHouses";

    void GenerateCrimsonVillage(GenerationProgress progress, GameConfiguration config)
    {
        if (!WorldGen.crimson)
        {
            return;
        }

        for (int i = 0; i < MultiStructureGenerator.GetStructureCount(CrimsonVillagePath, Mod); i++)
        {
            int attemptCounter = 0;
            Point16 structureDims = MultiStructureGenerator.GetStructureDimensions(CrimsonVillagePath, Mod, i);

            while (true)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceHigh);
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile ||
                    (tile.TileType != TileID.Crimstone && tile.TileType != TileID.CrimsonGrass && tile.TileType != TileID.Crimsand)
                    || Main.tile[x, y - 1].HasTile)
                {
                    attemptCounter++;
                    continue;
                }

                Rectangle structureRect = new Rectangle(x, y, structureDims.X, structureDims.Y);
                if (!GenVars.structures.CanPlace(structureRect, 5))
                {
                    attemptCounter++;
                    continue;
                }

                Point16 point = new Point16(x, y);
                MultiStructureGenerator.GenerateMultistructureSpecific(CrimsonVillagePath, i, point, Mod);
                GenVars.structures.AddProtectedStructure(structureRect, 5);
                break;
            }
        }
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int corruptionStepIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Tile Cleanup"));
        tasks.Insert(corruptionStepIndex + 1, new PassLegacy("Crimson Village", GenerateCrimsonVillage));
    }

    public override void PostUpdateWorld()
    {
        

        if (Main.keyState.IsKeyDown(Keys.B) && !Main.oldKeyState.IsKeyDown(Keys.B))
        {
            GenerateVerticalOceanTunnels();
            GenerateHorizontalOceanTunnels();
        }
    }

    void GenerateVerticalOceanTunnels()
    {
        int distanceBetweenTunnels = 50;
        int tunnelRepDistance = 20;
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
                int tunnelXPos = xStartPos + (amountOfTunnels * distanceBetweenTunnels);

                int yStartPos = ((int)Main.worldSurface - 100); // Ocean level isn't stored anywhere, literally just a guess
                int tunnelYPos = yStartPos + tunnelRepCount * tunnelRepDistance;

                WorldGen.digTunnel(tunnelXPos, tunnelYPos, 0, 3, 10, 8, true);
            }
        }
    }

    void GenerateHorizontalOceanTunnels()
    {
        int distanceBetweenTunnels = 75;
        int tunnelRepDistance = 20;
        //int repFailChanceDenominator = 4;
        for (int amountOfTunnels = 0; amountOfTunnels < 4; amountOfTunnels++)
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

                int yStartPos = ((int)Main.worldSurface - 50); // Ocean level isn't stored anywhere, literally just a guess
                int tunnelYPos = yStartPos + (amountOfTunnels * distanceBetweenTunnels);

                WorldGen.digTunnel(tunnelXPos, tunnelYPos, 3, 0, 10, 8, true);
            }
        }
    }
}
