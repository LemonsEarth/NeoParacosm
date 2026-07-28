using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class MeteorFragments : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 18;
        Item.crit = 0;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 32;
        Item.height = 30;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 10;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<MeteorFragment>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 trueVelocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-24, 24))) * Main.rand.NextFloat(0.8f, 1.10f);
                Projectile.NewProjectile(source, position, trueVelocity, type, damage, knockback, player.whoAmI);
            }
        }
        return false;
    }
}