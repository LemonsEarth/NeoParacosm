using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;
using NeoParacosm.Content.Items.Weapons.Melee;
using StructureHelper.API;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Ice;

public class IglooGenPass : GenPass
{
    public IglooGenPass(string name, float loadWeight) : base(name, loadWeight) { }

    readonly string IglooPath = "Common/Assets/Structures/Igloo";

    bool IsTileTypeSnowy(int tileType)
    {
        return tileType == TileID.IceBlock || tileType == TileID.CorruptIce || tileType == TileID.FleshIce || tileType == TileID.SnowBlock;
    }

    bool IsTileSnowy(Point point)
    {
        return Main.tile[point].HasTile && IsTileTypeSnowy(Main.tile[point].TileType);
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateIgloo();
    }

    public void GenerateIgloo()
    {
        Point16 structureDims = Generator.GetStructureDimensions(IglooPath, NeoParacosm.Instance);
        int startXTile = 400;
        int maxXTile = Main.maxTilesX - 400;
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

            if (Main.tile[pointTop].HasTile
                || (Main.tile[pointTopLeft].HasTile && Main.tile[pointTopRight].HasTile) 
                || !IsTileSnowy(pointBottomLeft) || !IsTileSnowy(pointBottomRight))
            {
                attemptCount++;
                continue;
            }

            Generator.GenerateStructure(IglooPath, new Point16(pointTopLeft), NeoParacosm.Instance);
            break;
        }

        List<List<(int, int)>> items =
        [
            [(ItemType<SnowgraveSpell>(), 1)],
            [(ItemID.SnowballLauncher, 1), (ItemID.IceSkates, 1)],
            [(ItemID.WarmthPotion, 3)],
            [(ItemID.Snowball, 40)],
            [(ItemID.BorealWood, 40)],
            [(ItemID.SnowBlock, 25)],
            [(ItemID.IceBlock, 25)]
        ];

        LemonUtils.GenerateStructureLoot(randX, randY, structureDims, items, 1f, 2f);
    }
}