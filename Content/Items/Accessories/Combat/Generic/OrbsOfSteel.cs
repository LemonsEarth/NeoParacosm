using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class OrbsOfSteel : ModItem
{
    readonly float critBoost = 16f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(critBoost);
    public override void SetDefaults()
    {
        Item.width = 46;
        Item.height = 40;
        Item.accessory = true;
        Item.defense = 6;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.HasAnyFireDebuff() || player.HasAnyPoisonDebuff())
        {
            player.GetCritChance(DamageClass.Generic) += critBoost;
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<PoisonOrb>(), 1);
        recipe.AddIngredient(ItemType<FireOrb>(), 1);
        recipe.AddRecipeGroup("NeoParacosm:AnyTitaniumBar", 8);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}
