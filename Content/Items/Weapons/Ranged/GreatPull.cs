using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class GreatPull : ModItem
{
    int useCount = 0;
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 80;
        Item.knockBack = 10f;
        Item.crit = 16;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 30;
        Item.height = 68;
        Item.useTime = 30;
        Item.useAnimation = 120;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.useAmmo = AmmoID.Arrow;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 20;
        Item.noMelee = true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.ItemAnimationJustStarted)
        {
            useCount = 0;
        }
        return null;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        //SoundEngine.PlaySound(SoundID.Item1, player.Center);
        useCount++;
        if (useCount >= 4)
        {
            SoundEngine.PlaySound(SFX.BowShot with { PitchRange = (-0.5f, -0.4f) }, player.Center);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }

    public override Vector2? HoldoutOffset()
    {
        return null;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<Greatbow>(), 1);
        recipe.AddIngredient(ItemID.MeteoriteBar, 10);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}

public class GreatPullArrow : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    bool isGreatPullArrow = false;
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (projectile.arrow && source is EntitySource_ItemUse_WithAmmo itemSource && itemSource.Item.type == ItemType<GreatPull>())
        {
            projectile.extraUpdates += 1;
            isGreatPullArrow = true;
        }
    }

    public override void OnKill(Projectile projectile, int timeLeft)
    {
        if (!isGreatPullArrow) return;
        SoundEngine.PlaySound(SoundID.Dig, projectile.position); // Plays the basic sound most projectiles make when hitting blocks.
        if (Main.myPlayer == projectile.owner)
        {
            LemonUtils.QuickProj(
                projectile,
                projectile.Center,
                Vector2.Zero,
                ProjectileType<GravitySuckyProjFriendly>(),
                ai0: 300,
                ai1: 40,
                ai2: 3
                );
        }
    }
}
