using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Underworld;

public class LavaCastleGenPass : GenPass
{
    public LavaCastleGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string LavaCastlePath = "Common/Assets/Structures/LavaCastle";

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateLavaCastle();
    }

    bool IsTileAsh(Point point)
    {
        return point.HasTile() && Main.tile[point].TileType == TileID.Ash;
    }

    public void GenerateLavaCastle()
    {
        Point16 structureDims = Generator.GetStructureDimensions(LavaCastlePath, NeoParacosm.Instance);
        int startXTile = 200;
        int maxXTile = Main.maxTilesX - 200;
        int startYTile = Main.maxTilesY - 200;
        int maxYTile = Main.maxTilesY - structureDims.Y * 3;

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
            Point pointTop = new Point(randX + structureDims.X / 2, randY);
            Point pointBottomLeft = new Point(randX, randY + structureDims.Y);
            Point pointBottomRight = new Point(randX + structureDims.X, randY + structureDims.Y);
            Point pointBottom = new Point(randX + structureDims.X / 2, randY + structureDims.Y);

            if (!IsTileAsh(pointBottomLeft) || !IsTileAsh(pointBottomRight) || !IsTileAsh(pointBottom))
            {
                attemptCount++;
                continue;
            }

            if (pointTop.HasTile())
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(LavaCastlePath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        if (attemptCount >= maxAttemptCount)
        {
            return;
        }

        List<List<(int, int)>> items =
        [
            [(ItemID.GoldPickaxe, 1), (ItemID.GoldAxe, 1)],
            [(ItemID.GoldBroadsword, 1), (ItemID.RubyStaff, 1)],
            [(ItemID.LavaCharm, 1), (ItemID.ObsidianRose, 1)],
            [(ItemID.LavaFishingHook, 1), (ItemID.SuperheatedBlood, 1)],
            [(ItemID.TreasureMagnet, 1)],
            [(ItemID.GoldBar, 10)],
            [(ItemID.Hellstone, 16)],
            [(ItemID.Obsidian, 20)],
            [(ItemID.DryBomb, 8)],
            [(ItemID.LavaBomb, 6)],
            [(ItemID.ObsidianSkinPotion, 2), (ItemID.InfernoPotion, 2)],
            [(ItemID.SpelunkerPotion, 2), (ItemID.HunterPotion, 2), (ItemID.TrapsightPotion, 2)],
            [(ItemID.HealingPotion, 5)],
            [(ItemID.GoldCoin, 8)],
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 1f, 2f);
    }
}