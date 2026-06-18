using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Items.Weapons.Melee;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class TheMalformer : ModItem
{
    int timer = 0;
    public override void SetDefaults()
    {
        Item.damage = 80;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 48;
        Item.height = 32;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Red;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<TheMalformerHeldProj>();
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
        LemonUtils.DrawDreadlordWeaponGlowInInventory(Type, position, scale, spriteBatch);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        LemonUtils.DrawDreadlordWeaponGlowInWorld(Item, rotation, scale, spriteBatch);
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        type = Item.shoot;
        position += new Vector2(velocity.SafeNormalize(Vector2.Zero).X * 18, -8);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<SupremeMusket>(), 1);
        recipe.AddIngredient(ItemType<SupremeUndertaker>(), 1);
        recipe.AddIngredient(ItemType<NightmareScale>(), 12);
        recipe.AddIngredient(ItemType<DivineFlesh>(), 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}