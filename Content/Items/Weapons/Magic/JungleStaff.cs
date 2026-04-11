using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class JungleStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 11;
        Item.DamageType = DamageClass.Magic;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 5;
        Item.useAnimation = 20;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 2);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item17;
        Item.autoReuse = true;
        Item.mana = 20;
        Item.shoot = ProjectileID.HornetStinger;
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.myPlayer == player.whoAmI && player.ItemAnimationJustStarted)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Main.MouseWorld + Vector2.UnitY.RotatedBy(i * MathHelper.PiOver4) * 80;
                Projectile.NewProjectileDirect(
                    source,
                    pos,
                    Vector2.UnitY.RotatedByRandom(6.28f) * 0.2f,
                    Main.rand.Next(ProjectileID.SporeGas, ProjectileID.SporeGas3 + 1),
                    damage,
                    0f,
                    player.whoAmI
                    );
            }
        }
        return true;
    }
}