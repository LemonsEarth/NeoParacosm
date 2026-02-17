using NeoParacosm.Content.Projectiles.Friendly.Ranged;

namespace NeoParacosm.Content.Items.Consumables;

public class GlowingMushroomSpore : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 14;
        Item.value = Item.buyPrice(0, 0, 0, 2);
        Item.rare = ItemRarityID.White;
        Item.ammo = Item.type;
        Item.shoot = ProjectileType<GlowingMushroomProj>();
        Item.consumable = true;
        Item.maxStack = 9999;
    }

    public override void AddRecipes()
    {
        CreateRecipe(20)
            .AddIngredient(ItemID.GlowingMushroom, 1)
            .AddCondition(Condition.TimeNight)
            .Register();
    }
}

public class GlowingMushroomGrassSeedsItem : GlobalItem
{
    public override void AddRecipes()
    {
        Recipe.Create(ItemID.MushroomGrassSeeds, 1)
            .AddIngredient(ItemType<GlowingMushroomSpore>(), 50)
            .AddTile(TileID.ChlorophyteExtractinator)
            .Register();
    }
}
