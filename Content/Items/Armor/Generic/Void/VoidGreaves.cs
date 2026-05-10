using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.Void;

[AutoloadEquip(EquipType.Legs)]
public class VoidGreaves : ModItem
{
    static readonly float moveSpeedBoost = 8;
    static readonly float critChanceBoost = 3;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(moveSpeedBoost, critChanceBoost);

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 14;
        //Item.defense = 10;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 2, 0, 0);
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
    {
        color = Color.White;
    }

    public override void UpdateEquip(Player player)
    {
        //player.moveSpeed += moveSpeedBoost / 100f;
        //player.GetCritChance(DamageClass.Generic) += critChanceBoost;
    }

    public override void AddRecipes()
    {

    }
}
