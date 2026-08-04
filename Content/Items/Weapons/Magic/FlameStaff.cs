using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class FlameStaff : ModItem
{
    float useCounter = 0;
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 8;
        Item.DamageType = DamageClass.Magic;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 3;
        Item.useAnimation = 15;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 0;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Yellow;
        Item.UseSound = SFX.Flamethrower;
        Item.autoReuse = true;
        Item.mana = 10;
        Item.shoot = ProjectileType<FlameStaffFlames>();
        Item.shootSpeed = 3;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: 60f);
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        float sinValue = MathF.Sin(useCounter) * MathHelper.Pi / 8f;
        velocity = velocity.RotatedBy(sinValue);
        position += velocity.SafeNormalize(Vector2.Zero) * 48f;
        useCounter += 1 / 5f;
    }
}