using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Movement;

public class CaptainsStormGear : ModItem
{
    bool spawnedLightning = false;
    bool spawnedLightningVertically = false;
    int timeSinceLastBurst = 0;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 44;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetJumpState<CaptainsStormGearExtraJump>().Enable();
        //player.jumpSpeedBoost += 5;

        //Main.NewText(player.velocity);
        //Main.NewText(timeSinceLastBurst);
        if (MathF.Abs(player.velocity.X) > 15 && !spawnedLightning)
        {
            LightningBurst(player, (timeSinceLastBurst / 90f + 1));
            if (timeSinceLastBurst < 180)
            {
                timeSinceLastBurst += 90;
            }
            spawnedLightning = true;
        }

        if (player.velocity.Y < -9 && !spawnedLightningVertically)
        {
            LightningBurst(player, (timeSinceLastBurst / 90f + 1));
            if (timeSinceLastBurst < 180)
            {
                timeSinceLastBurst += 90;
            }
            spawnedLightningVertically = true;
        }

        if (MathF.Abs(player.velocity.X) < 8)
        {
            spawnedLightning = false;
        }

        if (MathF.Abs(player.velocity.Y) < 8)
        {
            spawnedLightningVertically = false;
        }

        if (timeSinceLastBurst > 0) timeSinceLastBurst--;
    }

    public static void LightningBurst(Player player, float damageBoost = 1f)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = Vector2.UnitY.RotatedBy(i * MathHelper.PiOver4 + Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f));
                Projectile.NewProjectileDirect(
                    player.GetSource_FromThis(),
                    player.Center,
                    dir,
                    ProjectileType<HolyLightningFriendly>(),
                    damage: (int)(70 * damageBoost),
                    knockback: 15f,
                    player.whoAmI,
                    ai0: 0,
                    ai1: Main.rand.NextFloat(120, 180)
                    );
            }
        }
    }
}

public class CaptainsStormGearExtraJump : ExtraJump
{
    public override Position GetDefaultPosition() => new Before(CloudInABottle);

    public override float GetDurationMultiplier(Player player)
    {
        // Use this hook to set the duration of the extra jump
        // The XML summary for this hook mentions the values used by the vanilla extra jumps
        return 3.5f;
    }

    public override void UpdateHorizontalSpeeds(Player player)
    {
        // Use this hook to modify "player.runAcceleration" and "player.maxRunSpeed"
        // The XML summary for this hook mentions the values used by the vanilla extra jumps
        player.runAcceleration *= 3f;
        player.maxRunSpeed *= 1.5f;
    }

    public override void OnStarted(Player player, ref bool playSound)
    {
        CaptainsStormGear.LightningBurst(player);
    }

    public override void ShowVisuals(Player player)
    {
        Dust.NewDustPerfect(player.RandomPos(), DustType<FireDust>(), Vector2.Zero, newColor: Color.Black, Scale: 0.5f).noGravity = true;
        Dust.NewDustPerfect(player.RandomPos(), DustID.GemDiamond, Vector2.Zero, Scale: 1.5f, newColor: Color.White).noGravity = true;
    }
}
