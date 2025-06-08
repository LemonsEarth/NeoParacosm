using NeoParacosm.Common.Utils;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class ForestCrest : ModItem
{
    static readonly int sentryBoost = 1;
    static readonly float moveSpeedBoost = 5;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(sentryBoost, moveSpeedBoost);

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.maxTurrets += sentryBoost;
        player.moveSpeed += moveSpeedBoost / 100;
        player.NPAccessoryPlayer().forestCrest = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Wood, 20);
        recipe.AddIngredient(ItemID.Daybloom, 3);
        recipe.AddIngredient(ItemID.Sunflower, 1);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}
