using NeoParacosm.Content.Items.Placeable.Tiles;
using NeoParacosm.Content.Tiles.Depths;

namespace NeoParacosm.Content.Items.Placeable.Walls;

public class DepthStoneWallItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 400;
        //ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ParastoneWallUnsafeItem>();
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<DepthStoneWallBlock>());
        Item.width = 32;
        Item.height = 32;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe(4);
        recipe.AddIngredient(ModContent.ItemType<DepthStoneItem>());
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
