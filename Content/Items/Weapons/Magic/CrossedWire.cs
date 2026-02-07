using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class CrossedWire : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 36;
        Item.DamageType = DamageClass.Magic;
        Item.width = 44;
        Item.height = 44;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Pink;
        Item.UseSound = SoundID.Item8;
        Item.autoReuse = true;
        Item.mana = 4;
        Item.shoot = ProjectileType<CrossedWireHeldProj>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
        Item.channel = true;
        Item.noUseGraphic = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HallowedBar, 8)
            .AddIngredient(ItemID.SoulofMight, 12)
            .AddRecipeGroup(RecipeGroupID.IronBar, 12)
            .AddIngredient(ItemID.Wire, 20)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}