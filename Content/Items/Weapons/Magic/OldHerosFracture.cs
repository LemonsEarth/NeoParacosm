using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class OldHerosFracture : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 36;
        Item.DamageType = DamageClass.Magic;
        Item.width = 56;
        Item.height = 56;
        Item.useTime = 10;
        Item.useAnimation = 40;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 8;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item9;
        Item.autoReuse = true;
        Item.mana = 10;
        Item.shoot = ProjectileID.TerraBeam;
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item9, player.Center);
        Vector2 playerToMouse = player.Center.DirectionTo(Main.MouseWorld);
        Vector2 playerToMouseOrt = playerToMouse.RotatedBy(MathHelper.PiOver2);
        Vector2 pos = player.Center + -playerToMouse * Main.rand.NextFloat(48, 120) + playerToMouseOrt * Main.rand.NextFloat(-64, 64);
        Vector2 vel = pos.DirectionTo(Main.MouseWorld) * 25;
        Projectile.NewProjectileDirect(
            player.GetSource_FromThis("NeoParacosm:OldHerosFracture"),
            pos,
            vel,
            type,
            damage,
            knockback,
            player.whoAmI,
            ai1: 15f
            );
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.SkyFracture, 1);
        recipe.AddIngredient(ItemID.BrokenHeroSword, 1);
        recipe.AddIngredient(ItemID.SoulofMight, 5);
        recipe.AddIngredient(ItemID.SoulofSight, 5);
        recipe.AddIngredient(ItemID.SoulofFright, 5);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}

public class OldHerosFractureTerraBeam : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return lateInstantiation && entity.type == ProjectileID.TerraBeam;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source.Context == "NeoParacosm:OldHerosFracture")
        {
            projectile.DamageType = DamageClass.Magic;
        }
    }
}