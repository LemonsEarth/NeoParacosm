using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class AllForOneLocket : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 64;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        int maxHPBoost = 0;
        foreach (var p in Main.ActivePlayers)
        {
            if (maxHPBoost >= 100 || p.team == 0)
            {
                break;
            }
            if (p.team == player.team)
            {
                maxHPBoost += 20;
            }
        }

        player.statLifeMax2 += maxHPBoost;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Bone, 20);
        recipe.AddIngredient(ItemID.LifeCrystal, 3);
        recipe.AddRecipeGroup("NeoParacosm:AnyGoldBar", 10);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
