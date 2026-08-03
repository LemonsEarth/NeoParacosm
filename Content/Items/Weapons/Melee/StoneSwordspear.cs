using NeoParacosm.Content.Items.Weapons.Magic;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class StoneSwordspear : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 36;
        Item.DamageType = DamageClass.Melee;
        Item.width = 80;
        Item.height = 80;
        Item.useTime = 30;
        Item.useAnimation = 30;
        //Item.UseSound = SoundID.Item1 with { PitchRange = (-0.5f, -0.3f) };
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 4;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<StoneSwordspearHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
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
        int dir = LemonUtils.Sign(player.DirectionTo(Main.MouseWorld).X, 1);
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai0: dir);

        return false;
    }
}