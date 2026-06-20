using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class ForbiddenSnakeWhip : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToWhip(ProjectileType<ForbiddenSnakeWhipProjectile>(), 20, 2, 4);
        Item.rare = ItemRarityID.LightRed;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.GetModPlayer<ForbiddenSnakeWhipPlayer>().UseCount++;
        if (player.GetModPlayer<ForbiddenSnakeWhipPlayer>().UseCount >= 3)
        {
            player.GetModPlayer<ForbiddenSnakeWhipPlayer>().UseCount = 0;
        }
        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<SnakeWhip>());
        recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 3);
        recipe.AddIngredient(ItemID.SoulofNight, 6);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }

    public override bool MeleePrefix()
    {
        return true;
    }
}

public class ForbiddenSnakeWhipPlayer : ModPlayer
{
    public int UseCount { get; set; } = 0;
    public int HitCount { get; set; } = 0;

    public override void ResetEffects()
    {

    }
}

public class AncientStormFriendlyGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    bool spawnedByForbiddenSnakeWhip = false;
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.type == ProjectileID.SandnadoFriendly;
    }

    public override bool PreAI(Projectile projectile)
    {

        return true;
    }

    public override void PostAI(Projectile projectile)
    {
        if (spawnedByForbiddenSnakeWhip)
        {
            
        }
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source != null && source.Context == "ForbiddenSnakeWhip")
        {
            projectile.localAI[0] = 20;
            spawnedByForbiddenSnakeWhip = true;
        }
    }
}
