using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Magic;

public class AscendedCrimsonRod : ModItem
{
    int timer = 0;
    public override void SetDefaults()
    {
        Item.damage = 35;
        Item.DamageType = DamageClass.Magic;
        Item.width = 54;
        Item.height = 62;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.reuseDelay = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<AscendedCrimsonRodHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.mana = 6;
        Item.channel = true;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.CrimsonRod, position, scale, timer, frame, spriteBatch, Color.Yellow);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.CrimsonRod, rotation, scale, timer, spriteBatch, Color.Yellow);
        return false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        return false;
    }
}