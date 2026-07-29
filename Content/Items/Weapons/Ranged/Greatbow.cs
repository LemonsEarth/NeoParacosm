using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class Greatbow : ModItem
{
    int useCount = 0;
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 40;
        Item.knockBack = 16f;
        Item.crit = 16;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 26;
        Item.height = 64;
        Item.useTime = 30;
        Item.useAnimation = 120;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Blue;
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
        recipe.AddRecipeGroup(AnyRecipeGroups.AnyGoldBar, 15);
        recipe.AddIngredient(ItemID.Wood, 20);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}

public class GreatbowArrow : GlobalProjectile
{
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (projectile.arrow && source is EntitySource_ItemUse_WithAmmo itemSource && itemSource.Item.type == ItemType<Greatbow>())
        {
            projectile.penetrate += 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
            projectile.extraUpdates += 1;
        }
    }
}
