using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class BundleOfDarts : ModItem
{
    int useCounter = 0;

    public override void SetDefaults()
    {
        Item.damage = 9;
        Item.crit = 10;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<BundleOfDartsProj>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.consumable = false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 trueVelocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-24, 24))) * Main.rand.NextFloat(0.8f, 1.10f);
                Projectile.NewProjectile(source, position, trueVelocity, type, damage, knockback, player.whoAmI);
            }
        }
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.RichMahogany, 50);
        recipe1.AddIngredient(ItemID.Stinger, 9);
        //recipe1.AddIngredient(ItemID.SoulofFright, 5);
        //recipe1.AddIngredient(ItemID.AdamantiteBar, 6);
        recipe1.AddTile(TileID.WorkBenches);
        recipe1.Register();
    }
}