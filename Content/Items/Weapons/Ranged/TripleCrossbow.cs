
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class TripleCrossbow : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 28;
        Item.knockBack = 2f;
        Item.crit = 3;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 52;
        Item.height = 30;
        Item.useTime = 5;
        Item.useAnimation = 15;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.LightRed;
        Item.UseSound = SoundID.Item5;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.useAmmo = AmmoID.Arrow;
        Item.shootSpeed = 100;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item5 with { PitchRange = (0.1f, 0.2f) }, player.Center);
        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Wood, 12);
        recipe.AddRecipeGroup("NeoParacosm:AnyGoldBar", 15);
        recipe.AddIngredient(ItemID.Bone, 12);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
