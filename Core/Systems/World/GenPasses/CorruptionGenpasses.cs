using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Melee;
using StructureHelper.API;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses
{
    public class CorruptionGenpasses : GenPass
    {
        public CorruptionGenpasses(string name, float loadWeight) : base(name, loadWeight) { }

        readonly string CorruptBunkerPath = "Common/Assets/Structures/CorruptBunker";

        bool IsTileTypeCorrupt(int tileType)
        {
            return tileType == TileID.Ebonstone || tileType == TileID.CorruptGrass || tileType == TileID.Ebonsand;
        }

        bool IsTileCorrupt(Point point)
        {
            return Main.tile[point].HasTile && IsTileTypeCorrupt(Main.tile[point].TileType);
        }

        bool IsTileEbonstone(Point point)
        {
            return Main.tile[point].HasTile && Main.tile[point].TileType == TileID.Ebonstone;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            if (WorldGen.crimson)
            {
                return;
            }

            Point16 structureDims = Generator.GetStructureDimensions(CorruptBunkerPath, NeoParacosm.Instance);
            int startXTile = 200;
            int maxXTile = Main.maxTilesX - 200;
            int startYTile = (int)(Main.worldSurface);
            int maxYTile = ((int)Main.rockLayer);

            int maxAttemptCount = 1000000;
            int attemptCount = 0;
            while (attemptCount < maxAttemptCount)
            {
                int randX = Main.rand.Next(startXTile, maxXTile);
                int randY = Main.rand.Next(startYTile, maxYTile);
                Point pointTopLeft = new Point(randX, randY);
                Point pointTopRight = new Point(randX + structureDims.X, randY);
                Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
                Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);

                if (!IsTileEbonstone(pointTopLeft) || !IsTileEbonstone(pointTopRight) || !IsTileEbonstone(pointBottomLeft) || !IsTileEbonstone(pointBottomRight))
                {
                    attemptCount++;
                    continue;
                }

                Generator.GenerateStructure(CorruptBunkerPath, new Point16(pointTopLeft), NeoParacosm.Instance);
                break;
            }
        }
    }
}
