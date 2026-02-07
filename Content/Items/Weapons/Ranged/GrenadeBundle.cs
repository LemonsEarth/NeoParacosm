using NeoParacosm.Content.Items.Consumables;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class GrenadeBundle : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 36;
        Item.knockBack = 7f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 30;
        Item.height = 22;
        Item.useTime = 5;
        Item.useAnimation = 15;
        Item.reuseDelay = 45;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.Grenade;
        Item.shootSpeed = 10;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item1, player.Center);
        Projectile.NewProjectileDirect(Item.GetSource_FromThis("NeoParacosm:GrenadeBundle"), position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 16f, MathHelper.Pi / 16f));
        float rand = Main.rand.NextFloat(0, 1);
        if (rand > 0.5f)
        {
            type = ProjectileID.Grenade;
        }
        else if (rand >= 0.2f)
        {
            type = ProjectileID.StickyGrenade;
        }
        else
        {
            type = ProjectileID.BouncyGrenade;
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Grenade, 50)
            .AddIngredient(ItemID.StickyGrenade, 30)
            .AddIngredient(ItemID.StickyGrenade, 20)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

public class GrenadeTweak : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.type == ProjectileID.Grenade ||
            entity.type == ProjectileID.StickyGrenade ||
            entity.type == ProjectileID.BouncyGrenade;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source.Context == "NeoParacosm:GrenadeBundle")
        {
            projectile.hostile = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }
    }
}
