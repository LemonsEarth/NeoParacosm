using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using NeoParacosm.Content.Items.Weapons.Melee;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Corruption;

public class CorruptBunkerGenPass : GenPass
{
    public CorruptBunkerGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string CorruptBunkerPath = "Common/Assets/Structures/CorruptBunker";

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

            if (!IsTileEbonstone(pointTopLeft)
                || !IsTileEbonstone(pointTopRight)
                || !IsTileEbonstone(pointBottomLeft)
                || !IsTileEbonstone(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(CorruptBunkerPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemID.LuckyHorseshoe, 1), (ItemID.BandofRegeneration, 1), (ItemID.AdhesiveBandage, 1)],
            [(ItemID.DepthMeter, 1), (ItemID.AncientChisel, 1), (ItemID.Sextant, 1)],
            [(ItemID.GoldWatch, 1), (ItemID.PlatinumWatch, 1)],
            [(ItemID.HealingPotion, 3), (ItemID.LesserHealingPotion, 10)],
            [(ItemID.IronBroadsword, 1), (ItemID.SilverBroadsword, 1), (ItemID.GoldBroadsword, 1)],
            [(ItemID.LeadBow, 1), (ItemID.TungstenBow, 1), (ItemID.PlatinumBow, 1)],
            [(ItemID.LeadHelmet, 1), (ItemID.TungstenHelmet, 1), (ItemID.PlatinumHelmet, 1)],
            [(ItemID.IronChainmail, 1), (ItemID.SilverChainmail, 1), (ItemID.GoldChainmail, 1)],
            [(ItemID.LeadGreaves, 1), (ItemID.SilverGreaves, 1), (ItemID.PlatinumGreaves, 1)],
        ];
    }
}