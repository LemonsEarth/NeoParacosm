using NeoParacosm.Content.Projectiles.Friendly.Melee;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class Horizon : ModItem
{
    int useCounter = 0;
    public override void SetStaticDefaults()
    {
        //Item.staff[Type] = true;

    }

    public override void SetDefaults()
    {
        Item.damage = 120;
        Item.DamageType = DamageClass.Melee;
        Item.width = 82;
        Item.height = 82;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Red;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<HorizonProj>();
        Item.shootSpeed = 10;
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }

    public override void HoldItem(Player player)
    {

    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int dir = useCounter % 2 == 0 ? 1 : -1;
        Projectile.NewProjectileDirect(source, position, new Vector2(dir, useCounter), type, damage, knockback, player.whoAmI, ai0: 30, ai1: 20, ai2: -1);
        useCounter++;
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.PossessedHatchet, 1);
        recipe.AddIngredient(ItemID.FragmentSolar, 12);
        recipe.AddTile(TileID.LunarCraftingStation);
        recipe.Register();
    }
}