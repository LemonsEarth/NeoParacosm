using NeoParacosm.Common.Utils;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class RoundShield : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 30;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 0, 80);
        Item.rare = ItemRarityID.White;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.NPAccessoryPlayer().roundShield = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Wood, 15);
        recipe.AddRecipeGroup(RecipeGroupID.IronBar, 6);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}
