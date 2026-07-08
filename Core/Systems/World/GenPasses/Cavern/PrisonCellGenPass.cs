using NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Cavern;

public class PrisonCellGenPass : GenPass
{
    public PrisonCellGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string PrisonCellPath = "Common/Assets/Structures/PrisonCell";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < 5 * LemonUtils.GetWorldSize(); i++)
        {
            GeneratePrisonCell();
        }
    }

    public void GeneratePrisonCell()
    {
        Point16 structureDims = Generator.GetStructureDimensions(PrisonCellPath, NeoParacosm.Instance);
        int startXTile = 200;
        int maxXTile = Main.maxTilesX - 200;
        int startYTile = (int)(Main.rockLayer);
        int maxYTile = Main.maxTilesY - 400;

        int maxAttemptCount = 1000000;
        int attemptCount = 0;
        int randX = 0;
        int randY = 0;
        while (attemptCount < maxAttemptCount)
        {
            randX = WorldGen.genRand.Next(startXTile, maxXTile);
            randY = WorldGen.genRand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBelowBottomLeft = new Point(randX, randY + structureDims.Y + 1);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            Point pointBelowBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y + 1);

            if (!pointBelowBottomLeft.HasTile() || !pointBelowBottomRight.HasTile())
            {
                attemptCount++;
                continue;
            }
            if (pointTopLeft.HasTile() && pointTopRight.HasTile())
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(PrisonCellPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemID.BladedGlove, 1), (ItemID.BloodyMachete, 1), (ItemID.RedRyder, 1), (ItemID.StarAnise, 60), (ItemType<InvisibleManSpell>(), 1), (ItemID.SlimeStaff, 1)],
            [(ItemID.GasTrap, 1)],
            [(ItemID.EmptyBucket, 1)],
            [(ItemID.PoopBlock, 6)],
            [(ItemID.Bass, 3), (ItemID.Tuna, 3), (ItemID.Salmon, 3)],
            [(ItemID.Stinkfish, 3), (ItemID.Ebonkoi, 3), (ItemID.Hemopiranha, 3)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 1f, 1.5f);
    }
}