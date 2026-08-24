using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Tiles.Traps;

public class GravityTrap : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.DrawsWalls[Type] = true;
        TileID.Sets.DontDrawTileSliced[Type] = true;
        TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
        TileID.Sets.IsAMechanism[Type] = true;

        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.Width = 2;
        TileObjectData.newTile.Height = 2;
        TileObjectData.newTile.CoordinateHeights = [16, 16];
        TileObjectData.newTile.StyleHorizontal = false;
        TileObjectData.newTile.AnchorWall = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(129, 0, 230), Language.GetText("MapObject.Trap")); // localized text for "Trap"
    }


    public override bool IsTileDangerous(int i, int j, Player player) => true;

    public override bool CreateDust(int i, int j, ref int type)
    {
        type = DustID.Gold;
        return true;
    }

    public override void HitWire(int i, int j)
    {
        (int x, int y) = TileObjectData.TopLeft(i, j);
        // Wiring.CheckMech checks if the wiring cooldown has been reached. Put a longer number here for less frequent projectile spawns. 200 is the dart/flame cooldown. Spear is 90, spiky ball is 300
        if (Wiring.CheckMech(x, y, 180))
        {
            Tile tile = Main.tile[x, y];
            Vector2 spawnPosition = new Vector2(x * 16 + 16, y * 16 + 16);

            Projectile.NewProjectile(
                Wiring.GetProjectileSource(x, y),
                spawnPosition,
                Vector2.Zero,
                ProjectileType<GravityField>(),
                0,
                1f,
                -1,
                ai0: 60,
                ai1: 2f,
                ai2: 2f
                );

            const int TileWidth = 2;
            const int TileHeight = 2;

            // Here we call SkipWire on all tile coordinates covered by this tile. This ensures a wire signal won't run multiple times.
            for (int yy = y; yy < y + TileHeight; yy++)
            {
                for (int xx = x; xx < x + TileWidth; xx++)
                {
                    Wiring.SkipWire(xx, yy);
                }
            }
            //Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 20f, ProjectileID.PineNeedleHostile, 20, 2f, -1);
        }
    }
}

public class GravityTrapItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<GravityTrap>());
        Item.width = 16;
        Item.height = 16;
    }
}
