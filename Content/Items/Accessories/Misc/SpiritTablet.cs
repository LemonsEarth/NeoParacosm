using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class SpiritTablet : ModItem
{
    public static int LifeRestorationBoost { get; set; } = 30;
    public static int ManaRestorationBoost { get; set; } = 30;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeRestorationBoost, ManaRestorationBoost);

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 46;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<SpiritTabletPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemType<HeartTablet>());
        recipe1.AddIngredient(ItemType<StarTablet>());
        recipe1.AddTile(TileID.TinkerersWorkbench);
        recipe1.Register();
    }
}

public class SpiritTabletPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        if (Active)
        {
            healValue += SpiritTablet.LifeRestorationBoost;
        }
    }

    public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
    {
        if (Active)
        {
            healValue += SpiritTablet.ManaRestorationBoost;
        }
    }
}
