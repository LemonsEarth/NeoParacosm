using Microsoft.Xna.Framework;
using NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class BranchOfLife : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 44;
        Item.height = 44;
        Item.mana = 20;
        Item.noMelee = true;
        Item.damage = 77;
        Item.DamageType = DamageClass.Summon;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.UseSound = SoundID.Item44;
        Item.shoot = ProjectileType<BranchOfLifeSentry>();
        Item.rare = ItemRarityID.Expert;
        Item.value = 2000;
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
        recipe.AddIngredient(ItemType<PoisonBloomStaff>(), 1);
        recipe.AddIngredient(ItemType<BlinkrootBusterStaff>(), 1);
        recipe.AddIngredient(ItemType<WaterfallerStaff>(), 1);
        recipe.AddIngredient(ItemType<GiantShiverthornStaff>(), 1);
        recipe.AddIngredient(ItemType<MoonBurstStaff>(), 1);
        recipe.AddIngredient(ItemType<DeathseederStaff>(), 1);
        recipe.AddIngredient(ItemType<FireBloomStaff>(), 1);
        recipe.AddIngredient(ItemType<ShadowflowerStaff>(), 1);
        recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
        recipe.AddIngredient(ItemID.Ectoplasm, 10);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}
