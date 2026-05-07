using NeoParacosm.Content.Items.Consumables;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class MeatReaver : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 12;
        Item.knockBack = 5f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 72;
        Item.height = 36;
        Item.useTime = 10;
        Item.useAnimation = 50;
        Item.reuseDelay = 10;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 30);
        Item.rare = ItemRarityID.Pink;
        Item.shoot = ProjectileID.Bullet;
        Item.autoReuse = true;
        Item.useAmmo = AmmoID.Bullet;
        Item.shootSpeed = 20;
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
            LemonUtils.DustBurst(5, player.Center + velocity.SafeNormalize(Vector2.Zero) * 88, DustID.Crimslime, 3, 3, 2.2f, 2.3f);

            SoundEngine.PlaySound(SoundID.NPCDeath13, player.Center);

            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectileDirect(
                    source,
                    position,
                    velocity * (0.5f + i * 0.25f),
                    ProjectileID.Bone,
                    damage,
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
        recipe.AddIngredient(ItemType<FleshReaverHead>(), 1);
        recipe.AddIngredient(ItemID.ClockworkAssaultRifle, 1);
        recipe.AddIngredient(ItemID.Sandgun, 1);
        recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 3);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
