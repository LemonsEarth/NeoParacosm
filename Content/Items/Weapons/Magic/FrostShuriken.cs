using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class FrostShuriken : ModItem
{
    public override void SetStaticDefaults()
    {
        //Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 12;
        Item.DamageType = DamageClass.Magic;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.DD2_BetsysWrathShot with { PitchRange = (1f, 1.5f) };
        Item.autoReuse = true;
        Item.mana = 12;
        Item.shoot = ProjectileType<FrostShurikenProj>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {

        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<FrigidFossil>(), 10);
        recipe.AddIngredient(ItemID.ManaCrystal, 2);
        recipe.AddTile(TileID.IceMachine);
        recipe.Register();
    }
}