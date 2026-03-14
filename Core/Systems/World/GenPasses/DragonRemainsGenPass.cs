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

namespace NeoParacosm.Core.Systems.World.GenPasses
{
    public class DragonRemainsGenPass : GenPass
    {
        public DragonRemainsGenPass(string name, float loadWeight) : base(name, loadWeight) { }

        readonly string DragonRemainsPath = "Common/Assets/Structures/DragonRemainsSanctuary";

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int attemptCount = 0;
            int maxAttempts = 1000000;
            Point16 structureDims = Generator.GetStructureDimensions(DragonRemainsPath, NeoParacosm.Instance);
            while (attemptCount < maxAttempts)
            {
                bool conflict = false;
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.33f), (int)(Main.maxTilesX * 0.66f));
                int y = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, LemonUtils.UnderworldDepth - 100 - structureDims.Y);
                Rectangle structureRect = new Rectangle(x, y, structureDims.X, structureDims.Y);
                for (int i = 0; i < structureDims.X; i++)
                {
                    if (conflict) break;
                    for (int j = 0; j < structureDims.Y; j++)
                    {
                        Tile tile = Main.tile[x + i, y + j];
                        bool cantPlace = !GenVars.structures.CanPlace(structureRect, 0);
                        bool isSpecialBrick = LemonUtils.IsDungeonBrick(tile.TileType) || tile.TileType == TileID.LihzahrdBrick;
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
                    Generator.GenerateStructure(DragonRemainsPath, new Point16(x, y), NeoParacosm.Instance);
                    //Mod.Logger.Info($"Generated Dragon Remains At ({x}, {y})");
                    break;
                }
            }
        }
    }
}
