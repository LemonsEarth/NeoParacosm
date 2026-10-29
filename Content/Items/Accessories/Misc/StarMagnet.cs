using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class StarMagnet : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 54;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 0, 50);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<StarMagnetPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.FallenStar);
        recipe1.AddIngredient(ItemID.Topaz, 5);
        recipe1.AddRecipeGroup(RecipeGroupID.IronBar, 10);
        recipe1.AddTile(TileID.Anvils);
        recipe1.Register();
    }
}

public class StarMagnetPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }
}

public class StarMagnetGlobalItem : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return entity.type == ItemID.FallenStar;
    }

    public override void GrabRange(Item item, Player player, ref int grabRange)
    {
        if (player.GetModPlayer<StarMagnetPlayer>().Active)
        {
            grabRange += 16 * 16;
        }
    }
}
