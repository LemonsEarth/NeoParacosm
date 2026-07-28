using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class MeteorCore : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 30;
        Item.crit = 0;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 44;
        Item.height = 44;
        Item.useTime = 90;
        Item.useAnimation = 90;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 10;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Pink;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<MeteorCoreProjectile>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai0: 180);

        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<MeteorFragments>(), 300);
        recipe.AddIngredient(ItemID.HallowedBar, 8);
        recipe.AddIngredient(ItemID.SoulofLight, 10);
        recipe.AddIngredient(ItemID.SoulofSight, 5);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}