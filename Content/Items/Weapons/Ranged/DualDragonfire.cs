using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class DualDragonfire : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 40;
        Item.knockBack = 5f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 74;
        Item.height = 36;
        Item.useTime = 5;
        Item.useAnimation = 30;
        Item.reuseDelay = 0;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Red;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.useAmmo = AmmoID.Gel;
        Item.shootSpeed = 60;
        Item.noMelee = true;
        Item.consumeAmmoOnFirstShotOnly = true;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override Vector2? HoldoutOffset()
    {
        return null;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 pos = position + velocity.SafeNormalize(Vector2.Zero) * 74;
        for (int i = 0; i < 6; i++)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectileDirect(
                    source,
                    pos,
                    velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 8f, MathHelper.Pi / 8f)) * Main.rand.NextFloat(0.5f, 0.75f),
                    ProjectileType<IchorFlamethrowerFriendly>(),
                    damage,
                    knockback,
                    player.whoAmI,
                    ai0: 20,
                    ai1: 0.92f,
                    ai2: Main.rand.NextFloat(-MathHelper.Pi / 16f, MathHelper.Pi / 16f)
                    );
            }
            else
            {
                Projectile.NewProjectileDirect(
                    source,
                    pos,
                    velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 32f, MathHelper.Pi / 32f)) * Main.rand.NextFloat(0.75f, 1.25f),
                    ProjectileType<CursedFlamethrowerFriendly>(),
                    damage,
                    knockback,
                    player.whoAmI,
                    ai0: 20,
                    ai1: 0.95f,
                    ai2: Main.rand.NextFloat(-MathHelper.Pi / 32f, MathHelper.Pi / 32f)
                    );
            }
        }
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }


    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Flamethrower, 1);
        recipe.AddIngredient(ItemID.ElfMelter, 1);
        recipe.AddIngredient(ItemType<DivineFlesh>(), 10);
        recipe.AddIngredient(ItemType<NightmareScale>(), 10);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}
