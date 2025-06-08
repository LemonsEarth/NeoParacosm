using NeoParacosm.Common.Utils;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class CorruptedLifeCrystal : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 26;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.NPAccessoryPlayer().corruptedLifeCrystal = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.LifeCrystal);
        recipe1.AddRecipeGroup("NeoParacosm:AnyEvilMaterial", 8);
        recipe1.AddTile(TileID.Anvils);
        recipe1.Register();
    }
}
