using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.DeathKnight;

[AutoloadEquip(EquipType.Legs)]
public class DeathKnightGreaves : ModItem
{
    static readonly float moveSpeedBoost = 8;
    static readonly float critChanceBoost = 3;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(moveSpeedBoost, critChanceBoost);

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 18;
        Item.defense = 10;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 2, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed += moveSpeedBoost / 100f;
        player.GetCritChance(DamageClass.Generic) += critChanceBoost;
    }

    public override void AddRecipes()
    {

    }
}
