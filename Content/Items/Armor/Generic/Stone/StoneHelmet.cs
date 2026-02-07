using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.Stone;

[AutoloadEquip(EquipType.Head)]
public class StoneHelmet : ModItem
{
    static readonly float speedDecrease = 7;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(speedDecrease);
    public static LocalizedText setBonusText;

    public override void SetStaticDefaults()
    {
        setBonusText = this.GetLocalization("SetBonus");
    }

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 20;
        Item.defense = 5;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 10, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed -= speedDecrease / 100f;
    }

    public override void UpdateInventory(Player player)
    {
        
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemType<StoneBreastplate>() && legs.type == ItemType<StoneBoots>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = setBonusText.Value;
        player.NPArmorPlayer().StoneArmor = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.StoneBlock, 25);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
