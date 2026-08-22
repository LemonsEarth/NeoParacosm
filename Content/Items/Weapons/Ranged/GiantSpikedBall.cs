using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class GiantSpikedBall : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.SpikyBall);
        Item.width = 38;
        Item.height = 38;
        Item.damage = 30;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.shootSpeed = 2.5f;
        Item.shoot = ProjectileType<GiantSpikedBallProj>();
        Item.rare = ItemRarityID.Orange;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe(50);
        recipe.AddIngredient(ItemID.HellstoneBar, 2);
        recipe.AddIngredient(ItemID.SpikyBall, 50);
        recipe.AddTile(TileID.Hellforge);
        recipe.Register();
    }
}
