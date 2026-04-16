using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class ReinforcedTurtleShell : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetDefaults()
    {
        Item.width = 54;
        Item.height = 64;
        Item.accessory = true;
        Item.defense = 10;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.noKnockback = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.TurtleShell, 1);
        recipe.AddIngredient(ItemID.AdamantiteBar, 8);
        recipe.AddIngredient(ItemID.TitaniumBar, 8);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}