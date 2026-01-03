using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class FireOrb : ModItem
{
    readonly float critBoost = 12f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(critBoost);
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.HasAnyFireDebuff())
        {
            player.GetCritChance(DamageClass.Generic) += critBoost;
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Fireblossom, 3);
        recipe.AddIngredient(ItemID.HellstoneBar, 5);
        recipe.AddTile(TileID.Hellforge);
        recipe.Register();
    }
}
