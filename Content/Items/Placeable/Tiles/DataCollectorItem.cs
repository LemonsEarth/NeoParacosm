using NeoParacosm.Content.Tiles.Special;

namespace NeoParacosm.Content.Items.Placeable.Tiles;

public class DataCollectorItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Furnace;
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<DataCollectorTile>());
        Item.width = 40;
        Item.height = 40;
        Item.rare = ItemRarityID.Orange;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddRecipeGroup("NeoParacosm:AnyGoldBar", 12);
        recipe.AddRecipeGroup("NeoParacosm:AnyEvilMaterial", 10);
        recipe.AddIngredient(ItemID.Diamond, 3);
        recipe.AddIngredient(ItemID.Bone, 30);
        recipe.AddTile(TileID.HeavyWorkBench);
        recipe.Register();
    }
}
