using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class FrozenBoneBow : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 16;
        Item.knockBack = 6f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 34;
        Item.height = 48;
        Item.useTime = 50;
        Item.useAnimation = 50;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item5;
        Item.autoReuse = true;
        Item.useAmmo = AmmoID.Arrow;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        //SoundEngine.PlaySound(SoundID.Item1, player.Center);

        Projectile.NewProjectileDirect(Item.GetSource_FromThis("NeoParacosm:FrozenBoneBow"), position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        //velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 64, MathHelper.Pi / 64)) * Main.rand.NextFloat(1.1f, 1.25f);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-8, 0);
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<FrigidFossil>(), 10);
        recipe.AddRecipeGroup(AnyRecipeGroups.AnyGoldBar, 15);
        recipe.AddTile(TileID.IceMachine);
        recipe.Register();
    }
}

public class FrozenBoneBowProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public bool IsFrozenBoneBowProjectile { get; private set; } = false;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is not null && source.Context == "NeoParacosm:FrozenBoneBow")
        {
            IsFrozenBoneBowProjectile = true;
        }
    }
}

public class FrozenBoneBowGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public int hitCount = 0;

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.GetGlobalProjectile<FrozenBoneBowProjectile>().IsFrozenBoneBowProjectile)
        {
            hitCount++;
            LemonUtils.DustBurst(8, npc.Center, DustID.SnowflakeIce, 5, 5, 1.5f, 2.2f);

        }

        if (hitCount >= 5)
        {
            LemonUtils.DustBurst(8, npc.Center, DustID.SnowflakeIce, 5, 5, 2.5f, 4.2f);
            SoundEngine.PlaySound(SoundID.Item4, npc.Center);
            for (int i = 0; i < 6; i++)
            {
                Projectile.NewProjectileDirect(
                        projectile.GetSource_FromThis(),
                        npc.Center + new Vector2(Main.rand.NextFloat(-48, 48), Main.rand.NextFloat(-1000, -600)),
                        Vector2.UnitY * Main.rand.NextFloat(4, 8),
                        ProjectileID.NorthPoleSnowflake,
                        projectile.damage / 2,
                        projectile.knockBack / 2,
                        projectile.owner,
                        ai1: Main.rand.Next(0, 3)
                    );
            }
            hitCount = 0;
        }
    }
}
