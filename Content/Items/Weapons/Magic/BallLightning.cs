using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class BallLightning : ModItem
{
    public override void SetStaticDefaults()
    {
        //Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 34;
        Item.DamageType = DamageClass.Magic;
        Item.width = 74;
        Item.height = 58;
        Item.useTime = 150;
        Item.useAnimation = 150;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Yellow;
        Item.UseSound = SoundID.DD2_BetsysWrathShot with { PitchRange = (1f, 1.5f) };
        Item.autoReuse = true;
        Item.mana = 40;
        Item.shoot = ProjectileType<LightningBallFriendly>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(
            source,
            position,
            player.DirectionTo(Main.MouseWorld) * 5,
            type,
            damage,
            knockback,
            player.whoAmI,
            ai0: 60,
            ai1: 300,
            ai2: 120
            );
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }


    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.MagnetSphere, 1);
        recipe.AddIngredient(ItemType<KnightsLostSoul>(), 8);
        recipe.AddTile(TileID.Bookcases);
        recipe.Register();
    }
}