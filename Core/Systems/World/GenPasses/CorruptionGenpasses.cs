using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using NeoParacosm.Content.Items.Weapons.Melee;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses;

public class CorruptionGenpasses : GenPass
{
    public CorruptionGenpasses(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string CorruptBunkerPath = "Common/Assets/Structures/CorruptBunker";
    readonly string CorruptTowerPath = "Common/Assets/Structures/CorruptTower";

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

        GenerateCorruptBunker();
        GenerateCorruptTower();
    }

    public void GenerateCorruptBunker()
    {
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

    public void GenerateCorruptTower()
    {
        Point16 structureDims = Generator.GetStructureDimensions(CorruptTowerPath, NeoParacosm.Instance);
        int startXTile = 200;
        int maxXTile = Main.maxTilesX - 200;
        int startYTile = (int)(Main.worldSurface * 0.33f);
        int maxYTile = ((int)Main.worldSurface);

        int maxAttemptCount = 1000000;
        int attemptCount = 0;
        int randX = 0;
        int randY = 0;
        while (attemptCount < maxAttemptCount)
        {
            randX = Main.rand.Next(startXTile, maxXTile);
            randY = Main.rand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);

            if (Main.tile[pointTop].HasTile && Main.tile[pointTop].WallType == WallID.None || !IsTileCorrupt(pointBottomLeft) || !IsTileCorrupt(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(CorruptTowerPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        List<List<(int, int)>> items =
        [
            [(ItemType<CorruptStaff>(), 1)],
            [(ItemID.ShadowOrb, 1), (ItemID.BandofStarpower, 1)],
            [(ItemID.ManaCrystal, 3)],
            [(ItemID.MagicPowerPotion, 3), (ItemID.ManaRegenerationPotion, 3)],
            [(ItemID.MeteoriteBar, 5)],
            [(ItemID.TungstenBar, 15), (ItemID.SilverBar, 15)],
            [(ItemID.GoldCoin, 3)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 0.8f, 1.5f);
    }
}