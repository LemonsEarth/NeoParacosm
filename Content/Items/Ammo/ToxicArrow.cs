using NeoParacosm.Content.Projectiles.Friendly.Ranged;

namespace NeoParacosm.Content.Items.Ammo;

public class ToxicArrow : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 32;

        Item.damage = 8; // Keep in mind that the arrow's final damage is combined with the bow weapon damage.
        Item.DamageType = DamageClass.Ranged;
        Item.rare = ItemRarityID.Green;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 1f;
        Item.value = Item.sellPrice(copper: 16);
        Item.shoot = ProjectileType<ToxicArrowProjectile>(); // The projectile that weapons fire when using this item as ammunition.
        Item.shootSpeed = 8f; // The speed of the projectile.
        Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
    }

    // For a more detailed explanation of recipe creation, please go to Content/ExampleRecipes.cs.
    public override void AddRecipes()
    {
        CreateRecipe(50)
            .AddIngredient(ItemID.WoodenArrow, 50)
            .AddIngredient(ItemID.Stinger, 1)
            .AddIngredient(ItemID.JungleSpores, 1)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
