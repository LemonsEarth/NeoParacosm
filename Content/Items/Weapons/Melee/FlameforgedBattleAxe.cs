using NeoParacosm.Content.Projectiles.Friendly.Melee;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class FlameforgedBattleAxe : ModItem
{
    int useCounter = 0;
    public override void SetDefaults()
    {
        Item.damage = 36;
        Item.DamageType = DamageClass.Melee;
        Item.width = 80;
        Item.height = 80;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.LightRed;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<FlameforgedBattleAxeHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int direction = 1;
        if (useCounter % 2 != 0)
        {
            direction = -1;
        }
        int special = useCounter % 6 == 0 ? 1 : 0;
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai0: special, ai1: direction);
        useCounter++;
        return false;
    }
}