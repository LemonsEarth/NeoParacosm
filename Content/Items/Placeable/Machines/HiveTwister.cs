using NeoParacosm.Content.Items.Placeable.Special.ResearcherQuestTiles;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Machines;

// This file contains the tile, tile entity and item
public class HiveTwister : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;

        MinPick = 0;
        MineResist = 4f;
        AnimationFrameHeight = 90;
        DustType = DustID.Electric;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.UsesCustomCanPlace = true;
        //TileObjectData.newTile.StyleHorizontal = true;
        //TileObjectData.newTile.StyleWrapLimit = 15;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.Height = 4;
        TileObjectData.newTile.Width = 2;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = [ 16, 16, 16, 16 ];

        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.Origin = new Point16(0, 3);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 2, 0);
        TileObjectData.newTile.HookPostPlaceMyPlayer = GetInstance<HiveTwisterTileEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(229, 229, 98));
        AnimationFrameHeight = 72;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.9f;
        g = 0.9f;
        b = 0.9f;
    }

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        frameCounter++;
        if (frameCounter >= 24)
        {
            frameCounter = 0;
            frame++;
        }
        if (frame > 1)
        {
            frame = 0;
        }
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        GetInstance<HiveTwisterTileEntity>().Kill(i, j);
    }
}

public class HiveTwisterTileEntity : ModTileEntity
{
    int timer = 0;

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == TileType<HiveTwister>();
    }

    Point16 CenterPos => Position + new Point16(1, 2);
    float distance = 500;

    public override void Update()
    {
        if (timer % 300 == 0)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.Distance(Position.ToWorldCoordinates()) < distance && npc.CanBeChasedBy())
                {
                    if (Main.rand.NextBool(4))
                    {
                        Projectile.NewProjectile(
                        new EntitySource_TileEntity(this, "NeoParacosm:HiveTwisterSpawn"),
                        CenterPos.ToWorldCoordinates(),
                        Vector2.UnitY.RotatedByRandom(MathHelper.Pi * 2),
                        ProjectileID.GiantBee,
                        8,
                        1f,
                        -1
                        );
                    }
                    else
                    {
                        Projectile.NewProjectile(
                        new EntitySource_TileEntity(this, "NeoParacosm:HiveTwisterSpawn"),
                        CenterPos.ToWorldCoordinates(),
                        Vector2.UnitY.RotatedByRandom(MathHelper.Pi * 2),
                        ProjectileID.Bee,
                        8,
                        1f,
                        -1
                        );
                    }
                }
            }

        }

        timer++;
    }
}

public class HiveTwisterItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 20;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<HiveTwister>());
        Item.width = 32;
        Item.height = 64;
        Item.rare = ItemRarityID.Orange;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.BottledHoney, 3);
        recipe.AddIngredient(ItemID.BeeWax, 10);
        recipe.AddRecipeGroup("NeoParacosm:AnyGoldBar", 12);
        recipe.AddTile(TileID.HeavyWorkBench);
        recipe.Register();
    }
}
