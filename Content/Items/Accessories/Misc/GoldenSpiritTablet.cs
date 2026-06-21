using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class GoldenSpiritTablet : ModItem
{
    public static int LifeRestorationBoost { get; set; } = 50;
    public static int ManaRestorationBoost { get; set; } = 50;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeRestorationBoost, ManaRestorationBoost);

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 46;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<GoldenSpiritTabletPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemType<SpiritTablet>());
        recipe1.AddIngredient(ItemID.EyeoftheGolem);
        recipe1.AddTile(TileID.TinkerersWorkbench);
        recipe1.Register();
    }
}

public class GoldenSpiritTabletPlayer : ModPlayer
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
            if (Main.rand.NextBool(4))
            {
                healValue *= 2;
            }
            else
            {
                healValue += GoldenSpiritTablet.LifeRestorationBoost;
            }
        }
    }

    public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
    {
        if (Active)
        {
            if (Main.rand.NextBool(4))
            {
                healValue *= 2;
            }
            else
            {
                healValue += GoldenSpiritTablet.ManaRestorationBoost;
            }
        }
    }
}

public class GoldenSpiritTabletItem : GlobalItem
{
    public override bool? UseItem(Item item, Player player)
    {
        return null;
    }
}
