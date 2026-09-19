using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Particles;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

[AutoloadEquip(EquipType.Shield)]
public class LionsHolyShield : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 86;
        Item.height = 84;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.Yellow;
        Item.defense = 5;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.noKnockback = true;
        player.GetModPlayer<LionsHolyShieldPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.EoCShield, 1);
        recipe.AddIngredient(ItemType<EclipseGreatshield>(), 1);
        recipe.AddIngredient(ItemType<PaladinsHolyBarrier>(), 1);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}

public class LionsHolyShieldPlayer : ModPlayer
{
    int Timer { get; set; } = 0;
    public static int MaxTimer { get; set; } = 600;
    public static float MaxDR { get; set; } = 20;
    public bool Active { get; set; } = false;
    public bool BlockedByEclipseGreatshield { get; set; } = false;

    public const int DashRight = 2;
    public const int DashLeft = 3;

    public int DashCooldown = 50;
    public int DashDuration = 35;

    // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
    public float DashVelocity = 15f;

    // The direction the player has double tapped.  Defaults to -1 for no dash double tap
    public int DashDir = -1;

    public int DashDelay = 0; // frames remaining till we can dash again
    public int DashTimer = 0; // frames remaining in the dash

    bool alreadyHit = false;

    public override void ResetEffects()
    {
        Active = false;
        BlockedByEclipseGreatshield = false;

        if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15 && Player.doubleTapCardinalTimer[DashLeft] == 0)
        {
            DashDir = DashRight;
        }
        else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15 && Player.doubleTapCardinalTimer[DashRight] == 0)
        {
            DashDir = DashLeft;
        }
        else
        {
            DashDir = -1;
        }
    }

    public override void PreUpdateMovement()
    {
        if (CanUseDash() && DashDir != -1 && DashDelay == 0)
        {
            Vector2 newVelocity = Player.velocity;

            switch (DashDir)
            {
                case DashLeft when Player.velocity.X > -DashVelocity:
                case DashRight when Player.velocity.X < DashVelocity:
                    {
                        float dashDirection = DashDir == DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * DashVelocity;
                        break;
                    }
                default:
                    return;
            }

            DashDelay = DashCooldown;
            DashTimer = DashDuration;
            Player.velocity = newVelocity;
        }

        if (DashDelay > 0)
            DashDelay--;

        if (DashTimer > 0)
        {
            ParticleSystem.SpawnParticle(
                        ParticleID.Glowy,
                        Player.RandomPos(8, 8),
                        -Vector2.UnitY * Main.rand.NextFloat(0.5f, 3),
                        Color.LightYellow,
                        scale: 1f,
                        data0: 30,
                        data1: 5,
                        data2: 20,
                        data3: 0.93f);
            DashTimer--;
        }
    }

    private bool CanUseDash()
    {
        return Active
            && Player.dashType != DashID.TabiAndMasterNinjaGear // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
            && !Player.setSolar // player isn't wearing solar armor
            && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (DashTimer > 0 && !alreadyHit)
        {
            alreadyHit = true;
            SoundEngine.PlaySound(SoundID.DD2_BetsyHurt with { PitchRange = (-0.2f, 0.2f)}, Player.Center);
             Projectile.NewProjectileDirect(
                    Player.GetSource_FromThis(),
                    Player.Center,
                    Vector2.Zero,
                    ProjectileType<HolyRepelProjFriendly>(),
                    100, 0,
                    ai0: 200,
                    ai1: 6,
                    ai2: 3
                    );
        }
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        if (Active)
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
                        (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(200),
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

    public override void UpdateEquips()
    {
        if (DashTimer == 0)
        {
            alreadyHit = false;
        }
        if (Active)
        {
            if (Timer < MaxTimer)
            {
                Timer++;
            }

            if (NPPlayer.Timer % 10 == 0)
            {
                for (int i = 0; i < Timer / 150f; i++)
                {
                    ParticleSystem.SpawnParticle(
                        ParticleID.Glowy,
                        Player.RandomPos(8, 8),
                        -Vector2.UnitY * Main.rand.NextFloat(0.5f, 3),
                        Color.LightYellow,
                        scale: 1f,
                        data0: 30,
                        data1: 5,
                        data2: 20,
                        data3: 0.93f);
                }
            }

            Player.endurance += MathHelper.Lerp(0, MaxDR, (float)Timer / MaxTimer) / 100f;
        }
        else
        {
            Timer = 0;
        }
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Timer == MaxTimer)
        {
            Projectile.NewProjectileDirect(
                    Player.GetSource_FromThis(),
                    Player.Center,
                    Vector2.Zero,
                    ProjectileType<GreaterHolyRepelProjFriendly>(),
                    0, 0,
                    ai0: 250,
                    ai1: 10,
                    ai2: 3
                    );
        }
        Timer = 0;
    }
}
