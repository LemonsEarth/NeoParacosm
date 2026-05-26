using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class ClanNecklace : ModItem
{
    static readonly int minionBoost = 1;
    public static float NearbyDMGBoost { get; private set; } = 10;
    public static float NearbyRange { get; private set; } = 200;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(minionBoost, NearbyDMGBoost);

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ClanNecklacePlayer>().Active = true;
        if (!hideVisual)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust.NewDustPerfect(
                    player.Center - Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / 16f) + MathHelper.ToRadians((float)Main.timeForVisualEffects)) * NearbyRange,
                    DustID.GemDiamond,
                    Vector2.Zero
                    ).noGravity = true;
            }
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<ClanSigil>());
        recipe.AddIngredient(ItemID.PygmyNecklace);
        recipe.AddTile(TileID.TinkerersWorkbench);
        recipe.Register();
    }
}

public class ClanNecklacePlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }
}

public class ClanNecklaceGlobalProj : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return (entity.minion || entity.sentry || entity.IsMinionOrSentryRelated);
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        Player player = projectile.GetOwner();
        if (projectile.IsMinionOrSentryRelated && player.GetModPlayer<ClanNecklacePlayer>().Active && projectile.DistanceSQ(player.Center) < ClanNecklace.NearbyRange * ClanNecklace.NearbyRange)
        {
            projectile.damage = ((int)(projectile.originalDamage * (1 + ClanNecklace.NearbyDMGBoost / 100f)));
        }
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        Player player = projectile.GetOwner();
        if (player.GetModPlayer<ClanNecklacePlayer>().Active && projectile.DistanceSQ(player.Center) < ClanNecklace.NearbyRange * ClanNecklace.NearbyRange)
        {
            modifiers.FinalDamage *= (1 + ClanNecklace.NearbyDMGBoost / 100f);
        }
    }
}
