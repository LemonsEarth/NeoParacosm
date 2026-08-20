using NeoParacosm.Content.Projectiles.Friendly.Ranged;

namespace NeoParacosm.Content.Items.Ammo;

public class GravityArrow : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 36;

        Item.damage = 9; // Keep in mind that the arrow's final damage is combined with the bow weapon damage.
        Item.DamageType = DamageClass.Ranged;
        Item.rare = ItemRarityID.Green;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 1.5f;
        Item.value = Item.sellPrice(copper: 16);
        Item.shoot = ProjectileType<GravityArrowProjectile>(); // The projectile that weapons fire when using this item as ammunition.
        Item.shootSpeed = 7f; // The speed of the projectile.
        Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
    }

    // For a more detailed explanation of recipe creation, please go to Content/ExampleRecipes.cs.
    public override void AddRecipes()
    {
        CreateRecipe(25)
            .AddIngredient(ItemID.WoodenArrow, 25)
            .AddIngredient(ItemID.MeteoriteBar)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
