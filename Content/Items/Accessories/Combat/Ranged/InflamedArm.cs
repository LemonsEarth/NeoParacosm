using NeoParacosm.Content.Items.Accessories.Combat.Melee;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class InflamedArm : ModItem
{
    readonly float attackSpeedBoost = 25f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(attackSpeedBoost);
    public override void SetDefaults()
    {
        Item.width = 60;
        Item.height = 52;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 75);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetAttackSpeed(DamageClass.Ranged) += attackSpeedBoost / 100f;
        player.GetModPlayer<MoltenClockworkPlayer>().Active = true;
        player.pickSpeed -= 0.15f;
        player.tileSpeed += 0.15f;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<MechanicalArm>(), 1);
        recipe.AddIngredient(ItemType<MoltenClockwork>(), 1);
        recipe.AddTile(TileID.TinkerersWorkbench);
        recipe.Register();
    }
}
