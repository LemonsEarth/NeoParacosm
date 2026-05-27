using NeoParacosm.Common.RecipeGroups;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class OldWaysTalisman : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        for (int i = 0; i < 3; i++)
        {
            if (player.armor[i].IsAir)
            {
                player.maxMinions++;
            }
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddRecipeGroup(AnyRecipeGroups.AnyGoldBar, 10);
        recipe.AddIngredient(ItemID.Feather, 10);
        recipe.AddIngredient(ItemID.SunplateBlock, 20);
        recipe.AddIngredient(ItemID.Cloud, 50);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
