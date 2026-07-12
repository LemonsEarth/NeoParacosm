using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class CaptainsArsenal : ModItem
{
    int useCounter = 0;
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 64;
        Item.knockBack = 12f;
        Item.crit = 8;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 90;
        Item.height = 50;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.reuseDelay = 0;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<HolySpearFriendly>();
        Item.shootSpeed = 13;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item1, player.Center);

        for (int i = 0; i < 4; i++)
        {
            Projectile.NewProjectileDirect(
                source,
                position,
                velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 16, MathHelper.Pi / 16)) * Main.rand.NextFloat(1.1f, 1.25f),
                type, damage, knockback, player.whoAmI, ai0: 15, ai1: 0.3f, ai2: 30f);
        }
        if (useCounter % 5 == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectileDirect(
                    source,
                    position,
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 4f, MathHelper.Pi / 4f)) * Main.rand.NextFloat(10, 15f),
                    ProjectileType<DarkIncendiaryProjFriendly>(), damage, knockback, player.whoAmI);
            }
        }
        useCounter++;
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<HolySpears>(), 1);
        recipe.AddIngredient(ItemType<DarkIncendiary>(), 1);
        recipe.AddIngredient(ItemType<KnightsLostSoul>(), 8);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}
