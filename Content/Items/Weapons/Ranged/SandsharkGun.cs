using NeoParacosm.Content.Items.Materials;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class SandsharkGun : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 16;
        Item.knockBack = 5f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 72;
        Item.height = 36;
        Item.useTime = 5;
        Item.useAnimation = 25;
        Item.reuseDelay = 10;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 30);
        Item.rare = ItemRarityID.Pink;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.Bullet;
        Item.useAmmo = AmmoID.Bullet;
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override Vector2? HoldoutOffset()
    {
        return null;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item11, player.Center);
        if (Main.rand.NextBool(5))
        {
            LemonUtils.DustBurst(5, player.Center + velocity.SafeNormalize(Vector2.Zero) * 88, DustID.Sand, 3, 3, 2.2f, 2.3f);

            for (int i = 0; i < 1; i++)
            {
                Projectile.NewProjectileDirect(
                    source,
                    position,
                    velocity,
                    ProjectileID.SandBallGun,
                    damage * 2,
                    knockback,
                    player.whoAmI
                    );
            }
        }
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<SandSharkHead>(), 1);
        recipe.AddIngredient(ItemID.ClockworkAssaultRifle, 1);
        recipe.AddIngredient(ItemID.Sandgun, 1);
        recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 1);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
