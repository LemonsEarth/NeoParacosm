
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class TheStorm : ModItem
{
    int useCounter = 0;
    public override void SetDefaults()
    {
        Item.damage = 60;
        Item.knockBack = 8f;
        Item.crit = 3;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 48;
        Item.height = 62;
        Item.useTime = 10;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Yellow;
        Item.UseSound = SoundID.Item5;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.useAmmo = AmmoID.Arrow;
        Item.shootSpeed = 16;
        Item.noMelee = true;
    }

    public override Vector2? HoldoutOffset()
    {
        return null;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = -Vector2.UnitY * 20;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(Item.UseSound, player.Center);
        if (useCounter < 20) useCounter++;
        if (useCounter >= 20)
        {
            Projectile.NewProjectileDirect(
                source,
                player.Center + new Vector2(-player.direction * 1400, -900),
                new Vector2(player.direction * 60, 0),
                ProjectileType<TheStormProj>(),
                damage,
                knockback,
                player.whoAmI,
                ai0: player.direction,
                ai1: 0,
                ai2: 4
                );
            useCounter = 0;
        }

        for (int i = 0; i < 3; i++)
        {
            Projectile.NewProjectileDirect(
                source,
                Main.MouseWorld + new Vector2(Main.rand.NextFloat(-200, 200), Main.rand.NextFloat(-800, -700)),
                Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 12f, MathHelper.Pi / 12f)) * Item.shootSpeed,
                type,
                damage,
                knockback,
                player.whoAmI
                );
        }


        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<LamentOfTheLate>(), 1);
        recipe.AddIngredient(ItemID.DaedalusStormbow, 1);
        recipe.AddIngredient(ItemType<KnightsLostSoul>(), 8);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}
