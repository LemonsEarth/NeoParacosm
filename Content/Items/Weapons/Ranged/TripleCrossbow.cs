using Terraria.DataStructures;
using Terraria.ID;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class TripleCrossbow : ModItem
{
    int useCounter = 0;

    public override void SetDefaults()
    {
        Item.damage = 14;
        Item.knockBack = 2f;
        Item.crit = 3;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 114;
        Item.height = 60;
        Item.useTime = 15;
        Item.useAnimation = 45;
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
