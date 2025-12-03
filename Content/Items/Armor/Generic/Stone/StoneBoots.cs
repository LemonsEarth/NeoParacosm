using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.Stone;

[AutoloadEquip(EquipType.Legs)]
public class StoneBoots : ModItem
{
    static readonly float moveSpeedDecrease = 10;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(moveSpeedDecrease);

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 22;
        Item.defense = 6;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 10, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed -= moveSpeedDecrease / 100f;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.StoneBlock, 50);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
