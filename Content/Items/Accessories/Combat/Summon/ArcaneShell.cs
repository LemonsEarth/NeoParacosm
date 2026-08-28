using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class ArcaneShell : ModItem
{
    public static int MinionBoost { get; private set; } = 2;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionBoost);
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.LightPurple;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ArcaneStonePlayer>().Active = true;
        player.GetModPlayer<ArcaneShellPlayer>().Active = true;
        player.GetModPlayer<BloodArtistSkullPlayer>().Active = true;
        player.maxMinions += MinionBoost;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<ArcaneSkull>(), 1);
        recipe.AddIngredient(ItemType<BloodArtistSkull>(), 1);
        recipe.AddIngredient(ItemID.SoulofNight, 12);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}

public class ArcaneShellPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }
}

public class ArcaneShellMinionProjectile : GlobalProjectile
{
    public override void OnKill(Projectile projectile, int timeLeft)
    {
        if (projectile.minion && projectile.GetOwner().GetModPlayer<ArcaneShellPlayer>().Active)
        {
            projectile.GetOwner().AddBuff(BuffType<ArcaneSkullBuff>(), 900);
            if (Main.myPlayer == projectile.owner)
            {
                Player player = projectile.GetOwner();
                Vector2 toPlayer = projectile.DirectionTo(player.Center);
                int damage = projectile.damage;
                float knockback = projectile.knockBack;
                Projectile.NewProjectileDirect(
                        projectile.GetSource_Death("NeoParacosm:BloodArtistSkull"),
                        projectile.Center,
                        toPlayer * Main.rand.NextFloat(4, 6),
                        ProjectileType<LostSoulFriendly>(),
                        damage,
                        knockback,
                        player.whoAmI,
                        ai0: 30,
                        ai1: 300,
                        ai2: 2
                        );
            }
        }
    }
}
