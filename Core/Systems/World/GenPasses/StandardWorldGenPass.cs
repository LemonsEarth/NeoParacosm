using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses;

public class StandardWorldGenPass : GenPass
{
    //TODO: remove this once tML changes generation passes
    public StandardWorldGenPass() : base("Standard World", 100) { }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerationProgress cache = WorldGenerator.CurrentGenerationProgress; // Cache generation progress...
        WorldGen.GenerateWorld(Main.ActiveWorldFileData.Seed);
        WorldGenerator.CurrentGenerationProgress = cache; // ...because GenerateWorld sets it to null when it ends, and it must be set back
    }
}
