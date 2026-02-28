using NeoParacosm.Content.Items.Consumables;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class IceCometStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 30;
        Item.DamageType = DamageClass.Magic;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 10;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.mana = 36;
        Item.shoot = ProjectileType<IceComet>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        position = Main.MouseWorld + new Vector2(Main.rand.NextFloat(-600, -200), -1000);
        velocity = position.DirectionTo(Main.MouseWorld) * Item.shootSpeed;
        SoundEngine.PlaySound(SoundID.Item117 with { PitchRange = (0.5f, 0.75f) }, player.Center);
        Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 300, Main.MouseWorld.X, Main.MouseWorld.Y);
        return false;
    }
}