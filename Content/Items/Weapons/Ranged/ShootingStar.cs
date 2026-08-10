using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class ShootingStar : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 12;
        Item.knockBack = 6f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 34;
        Item.height = 48;
        Item.useTime = 50;
        Item.useAnimation = 50;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item5;
        Item.autoReuse = true;
        Item.useAmmo = AmmoID.Arrow;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        //SoundEngine.PlaySound(SoundID.Item1, player.Center);
        EntitySource_ItemUse_WithAmmo newSrc = new EntitySource_ItemUse_WithAmmo(source.Player, source.Item, source.AmmoItemIdUsed, "NeoParacosm:ShootingStar");
        Projectile.NewProjectileDirect(newSrc, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        //velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 64, MathHelper.Pi / 64)) * Main.rand.NextFloat(1.1f, 1.25f);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(0, 0);
    }
}

public class ShootingStarProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public bool IsShootingStarProjectile { get; private set; } = false;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is not null && source.Context == "NeoParacosm:ShootingStar")
        {
            IsShootingStarProjectile = true;
        }
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (IsShootingStarProjectile && hit.Crit)
        {
            LemonUtils.QuickProj(
                projectile,
                projectile.Center,
                projectile.GetOwner().DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)) * Main.rand.NextFloat(12, 16),
                ProjectileType<HomingStar>(),
                projectile.damage,
                3f,
                ai0: 45f,
                ai1: 120f
                );
        }
    }
}
