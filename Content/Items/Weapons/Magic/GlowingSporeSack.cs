using NeoParacosm.Content.Items.Consumables;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class GlowingSporeSack : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = false;
    }

    public override void SetDefaults()
    {
        Item.damage = 8;
        Item.DamageType = DamageClass.Magic;
        Item.width = 28;
        Item.height = 40;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item45;
        Item.autoReuse = true;
        Item.mana = 18;
        Item.shoot = ProjectileType<MushroomGas>();
        Item.useAmmo = ItemType<GlowingMushroomSpore>();
        Item.shootSpeed = 2;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        for (int i = 0; i < 2; i++)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 2, MathHelper.PiOver4 / 2)) * Main.rand.NextFloat(2f, 4f), ProjectileType<MushroomGas>(), damage, knockback, player.whoAmI);
            }
        }

        return false;
    }
}