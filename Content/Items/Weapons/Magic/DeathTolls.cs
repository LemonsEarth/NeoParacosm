using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class DeathTolls : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 25;
        Item.DamageType = DamageClass.Magic;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item8;
        Item.autoReuse = true;
        Item.mana = 36;
        Item.shoot = ProjectileType<DeathflameBallFriendly>();
        Item.shootSpeed = 5;
        Item.noMelee = true;
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        player.itemLocation += new Vector2(-8 * player.direction, 24);
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(ParacosmSFX.ChurchBell with { PitchRange = (-0.7f, -0.4f)}, player.Center);
        for (int i = 0; i < 3; i++)
        {
            Vector2 vel = -Vector2.UnitY.RotateRandom(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * velocity.Length();
            Projectile.NewProjectile(source, position, vel, type, damage, knockback, player.whoAmI, 60);
        }
        return false;
    }
}