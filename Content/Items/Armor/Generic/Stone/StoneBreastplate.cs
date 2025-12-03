using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.Stone;

[AutoloadEquip(EquipType.Body)]
public class StoneBreastplate : ModItem
{
    static readonly float speedDecrease = 7;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(speedDecrease);

    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 26;
        Item.defense = 9;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 10, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed -= speedDecrease / 100f;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.StoneBlock, 75);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
