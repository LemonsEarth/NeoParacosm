using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.DeathKnight;

[AutoloadEquip(EquipType.Body)]
public class DeathKnightChestplate : ModItem
{
    static readonly float damageBoost = 4;
    static readonly float critChanceBoost = 4;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critChanceBoost);

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 22;
        Item.defense = 17;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 3, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
        player.GetCritChance(DamageClass.Generic) += critChanceBoost;
    }

    public override void AddRecipes()
    {

    }
}
