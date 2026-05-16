using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Melee;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class FrostGauntlets : ModItem
{
    public override void SetStaticDefaults()
    {
        //Item.staff[Type] = true;
        
    }

    public override void SetDefaults()
    {
        Item.damage = 16;
        Item.DamageType = DamageClass.Melee;
        Item.width = 44;
        Item.height = 44;
        Item.useTime = 10;
        Item.useAnimation = 30;
        Item.reuseDelay = 15;
        Item.useStyle = ItemUseStyleID.Rapier;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        //Item.UseSound = SoundID.DD2_BetsysWrathShot with { PitchRange = (1f, 1.5f) };
        Item.autoReuse = true;
        Item.shoot = ProjectileType<FrostGauntletProj>();
        Item.shootSpeed = 10;
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot with { PitchRange = (2f, 2.5f) }, player.Center);
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f));
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<FrigidFossil>(), 10);
        recipe.AddRecipeGroup(AnyRecipeGroups.AnyGoldBar, 12);
        recipe.AddTile(TileID.IceMachine);
        recipe.Register();
    }
}