using NeoParacosm.Common.Utils;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class PoisonOrb : ModItem
{
    readonly float critBoost = 12f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(critBoost);
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.HasAnyPoisonDebuff())
        {
            player.GetCritChance(DamageClass.Generic) += critBoost;
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Stinger, 3);
        recipe.AddIngredient(ItemID.JungleSpores, 6);
        recipe.AddIngredient(ItemID.Vine, 2);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
