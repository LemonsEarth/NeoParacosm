using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Crimson;

public class CrimsonBunkerGenPass : GenPass
{
    public CrimsonBunkerGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string CrimsonBunkerPath = "Common/Assets/Structures/CrimsonBunker";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        if (!WorldGen.crimson)
        {
            return;
        }

        GenerateCrimsonBunker();
    }

    public void GenerateCrimsonBunker()
    {
        Point16 structureDims = Generator.GetStructureDimensions(CrimsonBunkerPath, NeoParacosm.Instance);
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
            randX = WorldGen.genRand.Next(startXTile, maxXTile);
            randY = WorldGen.genRand.Next(startYTile, maxYTile);
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointLeft = new Point(randX, randY + structureDims.Y / 2);
            Point pointRight = new Point(randX + structureDims.X, randY + structureDims.Y / 2);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);

            if (Main.tile[pointTop].HasTile || Main.tile[pointTop].WallType != WallID.None)
            {
                attemptCount++;
                continue;
            }

            if (!Main.tile[pointBottomLeft].HasTile || !Main.tile[pointBottomRight].HasTile || !Main.tile[pointLeft].HasTile || !Main.tile[pointRight].HasTile)
            {
                attemptCount++;
                continue;
            }

            if (!LemonUtils.IsTileCrimson(pointBottomLeft) && !LemonUtils.IsTileCrimson(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            if (!LemonUtils.IsTileCrimson(pointTopLeft) && !LemonUtils.IsTileCrimson(pointTopRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(CrimsonBunkerPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemID.LuckyHorseshoe, 1), (ItemID.BandofRegeneration, 1), (ItemID.AdhesiveBandage, 1)],
            [(ItemID.Compass, 1), (ItemID.Toolbox, 1), (ItemID.WeatherRadio, 1)],
            [(ItemID.GoldWatch, 1), (ItemID.PlatinumWatch, 1)],
            [(ItemID.HealingPotion, 3), (ItemID.LesserHealingPotion, 10)],
            [(ItemID.IronBar, 12), (ItemID.LeadBar, 12)],
            [(ItemID.GrayBrick, 40)],
            [(ItemID.Shadewood, 24)],
            [(ItemID.BottledWater, 4)],
            [(ItemID.Bottle, 40)],
            [(ItemID.GoldCoin, 1)],
            [(ItemID.IronHelmet, 1), (ItemID.SilverHelmet, 1), (ItemID.GoldHelmet, 1)],
            [(ItemID.LeadChainmail, 1), (ItemID.TungstenChainmail, 1), (ItemID.PlatinumChainmail, 1)],
            [(ItemID.IronGreaves, 1), (ItemID.TungstenGreaves, 1), (ItemID.GoldGreaves, 1)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 0.8f, 1.5f);
    }
}