using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class RotPerfume : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 6;
        Item.DamageType = DamageClass.Magic;
        Item.width = 64;
        Item.height = 64;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.reuseDelay = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item45;
        Item.autoReuse = true;
        Item.mana = 12;
        Item.shoot = ProjectileType<RotGas>();
        Item.shootSpeed = 2;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        for (int i = 0; i < 4; i++)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 2, MathHelper.PiOver4 / 2)) * Main.rand.NextFloat(2f, 4f), type, damage, knockback, player.whoAmI);
            }
        }

        return false;
    }
}