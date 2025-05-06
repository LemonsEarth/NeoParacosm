using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

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
}
