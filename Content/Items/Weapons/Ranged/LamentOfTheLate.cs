
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class LamentOfTheLate : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 28;
        Item.knockBack = 2f;
        Item.crit = 3;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 32;
        Item.height = 44;
        Item.useTime = 150;
        Item.useAnimation = 150;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item5;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.useAmmo = AmmoID.Arrow;
        Item.shootSpeed = 100;
        Item.noMelee = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = -Vector2.UnitY * 20;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.PickAmmo(Item, out int shotProjectile, out _, out _, out _, out _);
        Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ProjectileType<LamentOfTheLateProj>(), damage, 1f, player.whoAmI, shotProjectile, 60, 10);
        Projectile.NewProjectileDirect(source, position, velocity, shotProjectile, damage, knockback, player.whoAmI).extraUpdates = 1;

        return false;
    }
}
