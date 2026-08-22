using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class CursedMine : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.SpikyBall);
        Item.width = 38;
        Item.height = 38;
        Item.damage = 40;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.shootSpeed = 4f;
        Item.shoot = ProjectileType<CursedMineProj>();
        Item.rare = ItemRarityID.Pink;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe(50);
        recipe.AddIngredient(ItemType<GiantSpikedBall>(), 50);
        recipe.AddRecipeGroup(AnyRecipeGroups.AnyTitaniumBar, 2);
        recipe.AddIngredient(ItemID.CursedFlame, 2);
        recipe.AddIngredient(ItemID.SoulofSight, 1);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}
