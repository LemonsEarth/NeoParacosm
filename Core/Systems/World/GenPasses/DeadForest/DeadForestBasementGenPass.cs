using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.DeadForest;

public class DeadForestBasementGenPass : GenPass
{
    public DeadForestBasementGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string DeadForestBasementPath = "Common/Assets/Structures/DeadForestBasement";
    int baseDeadForestTileRadius = 200;
    int DeadForestRadius => baseDeadForestTileRadius * LemonUtils.GetWorldSize();

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateBasement();
    }

    void GenerateBasement()
    {
        Point16 structureDims = Generator.GetStructureDimensions(DeadForestBasementPath, NeoParacosm.Instance);

        int startXTile = (int)MathHelper.Clamp(Main.dungeonX - DeadForestRadius, 0, Main.maxTilesX);
        int maxXTile = (int)MathHelper.Clamp(Main.dungeonX + DeadForestRadius, 0, Main.maxTilesX);
        int startYTile = (int)MathHelper.Clamp(Main.dungeonY - 80, 0, Main.maxTilesY);
        int maxYTile = (int)MathHelper.Clamp(Main.dungeonY + 80, 0, Main.maxTilesY);

        int maxAttemptCount = 10000;
        int attemptCount = 0;
        int randX = 0;
        int randY = 0;
        while (attemptCount < maxAttemptCount)
        {
            randX = Main.rand.Next(startXTile, maxXTile);
            randY = Main.rand.Next(startYTile, maxYTile);
            Point pointTopLeft = new Point(randX, randY);
            Point pointTopRight = new Point(randX + structureDims.X, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);

            Point pointAboveTopLeft = new Point(randX, randY - 1);
            Point pointAboveTopRight = new Point(randX + structureDims.X, randY - 1);

            if (Main.tile[pointAboveTopLeft].HasTile && Main.tile[pointAboveTopRight].HasTile)
            {
                attemptCount++;
                continue;
            }

            if (!LemonUtils.IsTileDeadDirt(pointTopLeft)
                || !LemonUtils.IsTileDeadDirt(pointTopRight)
                || !LemonUtils.IsTileDeadDirt(pointBottomLeft)
                || !LemonUtils.IsTileDeadDirt(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(DeadForestBasementPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemType<DeathflameBallSpell>(), 1), (ItemType<RuneOfPeridition>(), 1)],
            [(ItemID.HealingPotion, 10), (ItemID.ManaPotion, 10)],
            [(ItemID.PoopBlock, 10)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 0.8f, 1.5f);
    }
}
