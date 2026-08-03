using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class AscendedUndertaker : AscendedGlowItem
{
    public override int OriginalItemID => ItemID.TheUndertaker;
    public override Color Color => Color.Yellow;

    public override void SetDefaults()
    {
        Item.damage = 24;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 50;
        Item.height = 28;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<AscendedUndertakerHeldProj>();
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