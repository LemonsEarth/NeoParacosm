using NeoParacosm.Content.Tiles.Special;

namespace NeoParacosm.Content.Items.Placeable.Tiles;

public class DataCollectorItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.StoneBlock;
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<DataCollectorTile>());
        Item.width = 80;
        Item.height = 80;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddRecipeGroup("NeoParacosm:AnyGoldBar", 12);
        recipe.AddRecipeGroup("NeoParacosm:AnyEvilMaterial", 10);
        recipe.AddIngredient(ItemID.Diamond, 3);
        recipe.AddTile(TileID.HeavyWorkBench);
        recipe.Register();
    }
}
