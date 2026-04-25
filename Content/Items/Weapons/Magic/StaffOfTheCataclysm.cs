using NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class StaffOfTheCataclysm : ModItem
{
    public static float Range { get; set; } = 800;
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 300;
        Item.DamageType = DamageClass.Magic;
        Item.width = 80;
        Item.height = 80;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item8;
        Item.autoReuse = true;
        Item.mana = 200;
        Item.shoot = ProjectileType<StaffOfTheCataclysmHeldProj>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
        Item.channel = true;
        Item.noUseGraphic = true;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] == 0;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai1: Range);
        return false;
    }
}