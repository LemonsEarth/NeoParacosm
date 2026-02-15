using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Content.Projectiles.Hostile.Death;

namespace NeoParacosm.Content.Items.Accessories.Combat;

[AutoloadEquip(EquipType.Shield)]
public class EclipseGreatshield : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 30;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
        Item.defense = 3;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<EclipseGreatshieldPlayer>().eclipseGreatshield = true;
    }
}

public class EclipseGreatshieldPlayer : ModPlayer
{
    public bool eclipseGreatshield { get; set; } = false;
    public bool blockedByEclipseGreatshield { get; set; } = false;
    public override void ResetEffects()
    {
        eclipseGreatshield = false;
        blockedByEclipseGreatshield = false;
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        if (eclipseGreatshield)
        {
            modifiers.Knockback *= 0;

            if (Main.rand.NextBool(4))
            {
                modifiers.FinalDamage *= 0.5f;
                LemonUtils.QuickPulse(Player, Player.MountedCenter, 2f, 3f, 5f, Color.Gold);
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectileDirect(
                        Player.GetSource_FromThis(),
                        Player.Center,
                        Player.DirectionTo(Main.MouseWorld) * (10 + 2 * i),
                        ProjectileType<HolyBlastFriendly>(),
                        (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(60),
                        2f,
                        Player.whoAmI,
                        0,
                        1.01f + 0.02f * i,
                        300
                        );
                }
            }
        }
    }
}
