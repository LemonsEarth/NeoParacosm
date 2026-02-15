using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class DarkIncendiary : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 26;
        Item.knockBack = 7f;
        Item.crit = 0;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 48;
        Item.height = 52;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.reuseDelay = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item1;
        Item.shoot = ProjectileType<DarkIncendiaryProjFriendly>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }
}
