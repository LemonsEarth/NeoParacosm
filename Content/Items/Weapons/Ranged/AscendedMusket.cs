using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class AscendedMusket : ModItem
{
    int timer = 0;
    public override void SetDefaults()
    {
        Item.damage = 53;
        Item.crit = 8;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 60;
        Item.height = 22;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Orange;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<AscendedMusketHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
        Item.useAmmo = AmmoID.Bullet;
    }

    public override bool CanUseItem(Player player)
    {
        return true;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.Musket, position, scale, timer, frame, spriteBatch, Color.YellowGreen);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.Musket, rotation, scale, timer, spriteBatch, Color.YellowGreen);
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        type = Item.shoot;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        return false;
    }
}