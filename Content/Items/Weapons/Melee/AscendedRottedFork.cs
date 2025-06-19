using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Projectiles.Friendly.Melee;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class AscendedRottedFork : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 36;
        Item.DamageType = DamageClass.Melee;
        Item.width = 52;
        Item.height = 58;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.reuseDelay = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<AscendedRottedForkHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, 3);
        return false;
    }
}