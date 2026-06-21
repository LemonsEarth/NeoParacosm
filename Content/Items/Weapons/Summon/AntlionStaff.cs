using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class AntlionStaff : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;
        Item.mana = 10;
        Item.noMelee = true;
        Item.damage = 20;
        Item.DamageType = DamageClass.Summon;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 90;
        Item.useAnimation = 90;
        Item.UseSound = SoundID.Item44;
        Item.shoot = ProjectileType<AntlionSentry>();
        Item.rare = ItemRarityID.Green;
        Item.value = 20000;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = Main.MouseWorld;
        damage = Item.damage;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            player.UpdateMaxTurrets();
            projectile.originalDamage = Item.damage;
        }
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Sandstone, 25);
        recipe.AddIngredient(ItemID.SandBlock, 50);
        recipe.AddIngredient(ItemID.AntlionMandible, 4);
        recipe.AddIngredient(ItemID.Amber, 5);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}
