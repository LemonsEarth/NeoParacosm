using NeoParacosm.Content.Projectiles.Friendly.Melee.HeldProjectiles;
using Terraria.Audio;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class StormStick : ModItem
{
    int useCounter = 0;
    int special = 0;
    public override void SetStaticDefaults()
    {
        //Main.RegisterItemAnimation(Type, new DrawAnimationVertical(30, 2, false));
        //ItemID.Sets.AnimatesAsSoul[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 84;
        Item.DamageType = DamageClass.Melee;
        Item.width = 112;
        Item.height = 112;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(gold: 20);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.channel = true;
        Item.shoot = ProjectileType<StormStickHeldProj>();
        //Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (0.3f, 0.6f) }, player.Center);
        int direction = 1;
        if (useCounter % 2 != 0)
        {
            direction = -1;
        }

        if (useCounter % 3 == 0 && useCounter > 0)
        {
            special = 1;
        }
        else
        {
            special = 0;
        }
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai0: special, ai1: direction, ai2: useCounter);
        useCounter++;
        return false;
    }
}