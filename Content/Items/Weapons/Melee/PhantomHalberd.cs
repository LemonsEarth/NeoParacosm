using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class PhantomHalberd : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 110;
        Item.DamageType = DamageClass.Melee;
        Item.width = 64;
        Item.height = 64;
        Item.useTime = 16;
        Item.useAnimation = 16;
        Item.UseSound = SoundID.Item71;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<PhantomHalberdHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool? UseItem(Player player)
    {
        return null;
    }

    public override void UpdateInventory(Player player)
    {

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
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai0: 20);
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.SpectreBar, 10);
        recipe.AddIngredient(ItemID.Ectoplasm, 15);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}