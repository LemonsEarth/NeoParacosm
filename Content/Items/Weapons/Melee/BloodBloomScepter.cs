using NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;
using NeoParacosm.Content.Projectiles.Friendly.Melee.HeldProjectiles;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class BloodBloomScepter : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 36;
        Item.DamageType = DamageClass.Melee;
        Item.width = 128;
        Item.height = 128;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 12;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<BloodBloomHeldProjMelee>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<SunflowerScepter>())
            .AddRecipeGroup("NeoParacosm:AnyEvilMaterial", 12)
            .AddIngredient(ItemID.Deathweed, 3)
            .AddTile(TileID.DemonAltar)
            .AddCondition(Condition.BloodMoon)
            .Register();
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
        if (player.altFunctionUse == 2)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<BloodBloomHeldProjMagic>(), (int)player.GetTotalDamage(DamageClass.Magic).ApplyTo(Item.damage), knockback, player.whoAmI);
        }
        else
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        }
        return false;
    }
}