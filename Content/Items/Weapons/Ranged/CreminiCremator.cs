using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;
using Terraria.GameContent;
namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class CreminiCremator : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 720;
        Item.crit = 12;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 140;
        Item.height = 40;
        Item.useTime = 120;
        Item.useAnimation = 120;
        Item.UseSound = null;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 10);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<CreminiCrematorHeldProj>();
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
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, null, Color.White, 0f, TextureAssets.Item[Type].Size() * 0.5f, scale * 1.5f, SpriteEffects.None, 0);
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

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SniperRifle)
            .AddIngredient(ItemID.ShroomiteBar, 12)
            .AddIngredient(ItemID.Ectoplasm, 8)
            .AddTile(TileID.Autohammer)
            .Register();
    }
}