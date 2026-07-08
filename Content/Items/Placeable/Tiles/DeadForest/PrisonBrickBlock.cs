using NeoParacosm.Common.RecipeGroups;

namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class PrisonBrickBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBrick[Type] = true;
        //Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        HitSound = SoundID.Dig;
        MinPick = 55;
        MineResist = 2;
        DustType = DustID.Dirt;
        Main.tileMerge[TileType<DeadDirtBlock>()][Type] = true;
        TileID.Sets.ChecksForMerge[Type] = true;
        AddMapEntry(new Color(80, 80, 80));
    }

    public override void RandomUpdate(int i, int j)
    {

    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}

public class PrisonBrickItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.GrayBrick;
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<PrisonBrickBlock>());
        Item.width = 16;
        Item.height = 16;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GrayBrick, 50)
            .AddRecipeGroup(AnyRecipeGroups.AnyGoldBar)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
