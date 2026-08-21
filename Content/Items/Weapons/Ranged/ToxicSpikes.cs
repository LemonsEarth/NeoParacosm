using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class ToxicSpikes : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.SpikyBall);
        Item.shoot = ProjectileType<ToxicSpikesProj>();
        Item.rare = ItemRarityID.Green;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {

        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe(150);
        recipe.AddIngredient(ItemID.SpikyBall, 150);
        recipe.AddIngredient(ItemID.Stinger, 3);
        recipe.AddIngredient(ItemID.JungleSpores, 2);
        recipe.AddIngredient(ItemType<PureLifeEnergy>(), 2);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
