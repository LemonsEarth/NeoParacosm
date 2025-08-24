



using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class AshenNatureScepter : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 18;
        Item.DamageType = DamageClass.Magic;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Orange;
        Item.UseSound = SoundID.Item8;
        Item.autoReuse = true;
        Item.mana = 12;
        Item.shoot = ProjectileType<AshenBolt>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 120, ai2: 1);
        }
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<NatureScepter>());
        recipe.AddIngredient(ItemID.HellstoneBar, 10);
        recipe.AddTile(TileID.Hellforge);
        recipe.Register();
    }
}