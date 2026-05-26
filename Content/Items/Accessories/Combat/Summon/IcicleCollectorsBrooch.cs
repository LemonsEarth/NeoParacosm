using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class IcicleCollectorsBrooch : ModItem
{
    int timer = 0;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.whoAmI == Main.myPlayer && timer % 60 == 0)
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner == player.whoAmI && proj.minion)
                {
                    Projectile.NewProjectileDirect(
                        player.GetSource_FromThis(),
                        proj.Center,
                        -Vector2.UnitY * proj.velocity.Length() / 3f,
                        ProjectileID.NorthPoleSnowflake,
                        proj.damage / 2,
                        proj.knockBack / 2,
                        player.whoAmI,
                        ai1: Main.rand.Next(0, 3)
                    );
                }
            }
        }

        timer++;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<FrigidFossil>(), 8);
        recipe.AddRecipeGroup(AnyRecipeGroups.AnyGoldBar, 10);
        recipe.AddTile(TileID.IceMachine);
        recipe.Register();
    }
}
