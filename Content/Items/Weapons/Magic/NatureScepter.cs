



using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class NatureScepter : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 11;
        Item.DamageType = DamageClass.Magic;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 5;
        Item.useAnimation = 15;
        Item.reuseDelay = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item8;
        Item.autoReuse = true;
        Item.mana = 12;
        Item.shoot = ProjectileType<NatureBolt>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            Vector2 vel = velocity.RotatedByRandom(6.28f);
            Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.Zero) * 48, vel, type, damage, knockback, player.whoAmI, 60);
        }
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.JungleSpores, 10);
        recipe.AddIngredient(ItemID.Stinger, 7);
        recipe.AddIngredient(ItemID.Vine, 4);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}