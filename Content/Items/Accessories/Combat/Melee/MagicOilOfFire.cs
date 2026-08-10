using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class MagicOilOfFire : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 38;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddBuff(BuffID.WeaponImbueFire, 2);
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.BottledWater, 1);
        recipe.AddIngredient(ItemID.Hellstone, 30);
        recipe.AddTile(TileID.AlchemyTable);
        recipe.Register();
    }
}