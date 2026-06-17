using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Melee;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class FleshTwister : ModItem
{
    int useCounter = 0;
    public override void SetDefaults()
    {
        Item.damage = 150;
        Item.DamageType = DamageClass.Melee;
        Item.width = 102;
        Item.height = 60;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ItemRarityID.Red;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<FleshTwisterHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool AltFunctionUse(Player player)
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

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {

        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<SupremeBallOHurt>(), 1);
        recipe.AddIngredient(ItemType<SupremeRottedFork>(), 1);
        recipe.AddIngredient(ItemType<NightmareScale>(), 12);
        recipe.AddIngredient(ItemType<DivineFlesh>(), 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}