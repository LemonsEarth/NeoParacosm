using NeoParacosm.Content.Projectiles.Friendly.Melee;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class ChainsawGun : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 18;
        Item.crit = 4;
        Item.DamageType = DamageClass.Melee;
        Item.width = 48;
        Item.height = 42;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.reuseDelay = 30;
        Item.UseSound = SoundID.Item22;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<ChainsawGunHeldProjectile>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
    }
}