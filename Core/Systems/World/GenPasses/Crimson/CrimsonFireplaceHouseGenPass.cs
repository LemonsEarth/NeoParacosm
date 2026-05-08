using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using NeoParacosm.Content.Items.Weapons.Melee;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Crimson;

public class CrimsonFireplaceHouseGenPass : GenPass
{
    public CrimsonFireplaceHouseGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string CrimsonFireplaceHousePath = "Common/Assets/Structures/CrimsonFireplaceHouse";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        if (!WorldGen.crimson)
        {
            return;
        }

        GenerateCrimsonFireplaceHouse();
    }

    public void GenerateCrimsonFireplaceHouse()
    {
        Point16 structureDims = Generator.GetStructureDimensions(CrimsonFireplaceHousePath, NeoParacosm.Instance);
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

            if (Main.tile[pointTop].HasTile || Main.tile[pointTop].WallType != WallID.None
                || !LemonUtils.IsTileCrimson(pointBottomLeft) || !LemonUtils.IsTileCrimson(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(CrimsonFireplaceHousePath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemType<ChainsawGun>(), 1)],
            [(ItemID.CrimsonHeart, 1), (ItemID.PanicNecklace, 1)],
            [(ItemID.LifeCrystal, 3)],
            [(ItemID.RagePotion, 3), (ItemID.WrathPotion, 3)],
            [(ItemID.CrimtaneBar, 12)],
            [(ItemID.TungstenBar, 6), (ItemID.SilverBar, 8)],
            [(ItemID.GoldCoin, 5)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 0.8f, 2f);
    }
}