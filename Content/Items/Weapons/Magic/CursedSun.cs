using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class CursedSun : ModItem
{
    public override void SetStaticDefaults()
    {
        //Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 180;
        Item.DamageType = DamageClass.Magic;
        Item.width = 54;
        Item.height = 52;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Red;
        Item.UseSound = SoundID.DD2_BetsysWrathShot with { PitchRange = (1f, 1.5f) };
        Item.autoReuse = true;
        Item.mana = 30;
        Item.shoot = ProjectileType<GiantCursedFlameSphereFriendly>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(
            source,
            position,
            player.DirectionTo(Main.MouseWorld) * 15,
            type,
            damage,
            knockback,
            player.whoAmI,
            ai0: 120,
            ai1: 1.02f,
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
        recipe.AddIngredient(ItemID.CursedFlames, 1);
        recipe.AddIngredient(ItemType<NightmareScale>(), 10);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}