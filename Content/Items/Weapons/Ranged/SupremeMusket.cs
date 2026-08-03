using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class SupremeMusket : AscendedGlowItem
{
    public override int OriginalItemID => ItemID.Musket;
    public override Color Color => Color.Magenta;

    public override void SetDefaults()
    {
        Item.damage = 40;
        Item.crit = 16;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 60;
        Item.height = 22;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Pink;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<SupremeMusketHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
        Item.useAmmo = AmmoID.Bullet;
    }

    public override bool CanUseItem(Player player)
    {
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        type = Item.shoot;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        return false;
    }
}