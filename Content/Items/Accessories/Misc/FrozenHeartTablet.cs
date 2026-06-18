using NeoParacosm.Content.Items.Materials;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class FrozenHeartTablet : ModItem
{
    public static int RestorationBoost { get; set; } = 25;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RestorationBoost);
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 46;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Blue;
        Item.defense = 2;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FrozenHeartTabletPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.LifeCrystal);
        recipe1.AddIngredient(ItemID.StoneBlock, 25);
        recipe1.AddIngredient(ItemType<FrigidFossil>(), 5);
        recipe1.AddTile(TileID.Anvils);
        recipe1.Register();
    }
}

public class FrozenHeartTabletPlayer : ModPlayer
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
            healValue += FrozenHeartTablet.RestorationBoost;
        }
    }
}
