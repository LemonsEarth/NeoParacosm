using NeoParacosm.Content.Tiles.Special;

namespace NeoParacosm.Content.Items.Placeable.Tiles;

public class DataCollectorEXItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Furnace;
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<DataCollectorEXTile>());
        Item.width = 40;
        Item.height = 40;
        Item.rare = ItemRarityID.LightRed;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ModContent.ItemType<DataCollectorItem>(), 1);
        recipe.AddRecipeGroup("NeoParacosm:AnyTitaniumBar", 12);
        recipe.AddRecipeGroup("NeoParacosm:AnyMythrilBar", 6);
        recipe.AddRecipeGroup("NeoParacosm:AnyEvilMaterial2", 10);
        recipe.AddIngredient(ItemID.PixieDust, 15);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}
