using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Melee;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class SpaceShredder : ModItem
{
    public override void SetStaticDefaults()
    {
        //Item.staff[Type] = true;

    }

    public override void SetDefaults()
    {
        Item.damage = 100;
        Item.DamageType = DamageClass.Magic;
        Item.width = 78;
        Item.height = 72;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 10;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ItemRarityID.Red;
        Item.UseSound = SoundID.Item92;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<SpaceShredderPlanet>();
        Item.shootSpeed = 10;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.channel = true;
        Item.mana = 25;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] < 5;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(source, position, velocity * 3, type, damage, knockback, player.whoAmI,
            ai0: Main.rand.NextFloat(0.8f, 1.25f), ai1: LemonUtils.Sign(player.DirectionTo(Main.MouseWorld).X, 1), ai2: player.ownedProjectileCounts[Item.shoot]);
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.MeteorStaff);
        recipe.AddIngredient(ItemType<Equinox>());
        recipe.AddIngredient(ItemID.FragmentNebula, 12);
        recipe.AddTile(TileID.LunarCraftingStation);
        recipe.Register();
    }
}