using NeoParacosm.Content.Projectiles.Friendly.Melee;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class ChainsawGun : ModItem
{
    int useCounter = 0;

    public override void SetDefaults()
    {
        Item.damage = 18;
        Item.crit = 10;
        Item.DamageType = DamageClass.Melee;
        Item.width = 48;
        Item.height = 42;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.reuseDelay = 30;
        Item.UseSound = SoundID.Item22;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<ChainsawGunHeldProjectile>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        //recipe1.AddIngredient(ItemID.RichMahogany, 50);
        //recipe1.AddIngredient(ItemID.Stinger, 9);
        //recipe1.AddIngredient(ItemID.SoulofFright, 5);
        //recipe1.AddIngredient(ItemID.AdamantiteBar, 6);
        //recipe1.AddTile(TileID.WorkBenches);
        recipe1.Register();
    }
}