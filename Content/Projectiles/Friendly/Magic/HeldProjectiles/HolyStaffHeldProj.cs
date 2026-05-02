using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;
namespace NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;

public class HolyStaffHeldProj : ModProjectile
{
    int AITimer = 0;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 28;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    int attackCount = 0;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }

        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item20 with { Volume = 0.8f, PitchRange = (-0.4f, -0.2f) }, Projectile.Center);
        }

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        SetPositionRotationDirection(player, player.Center.DirectionTo(Main.MouseWorld).ToRotation());

        int attackInterval = (int)(10f / MathHelper.Clamp(player.GetAttackSpeed(DamageClass.Magic), 0.1f, 5f));
        if (AITimer % attackInterval == 0)
        {
            SoundEngine.PlaySound(SoundID.Item92 with { PitchRange = (2f, 2.3f), Volume = 0.75f }, Projectile.Center);
            attackCount++;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromAI(),
                    Projectile.Center,
                    Projectile.DirectionTo(Main.MouseWorld) * 1,
                    ProjectileType<HolyBlastFriendly>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    ai1: 1.005f + attackCount * 0.01f,
                    ai2: 300
                    );
            }
        }


        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 5f, DustID.GemTopaz, 1.5f);
    }

    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 dir = player.Center.DirectionTo(Main.MouseWorld);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 0);
        Projectile.Center = player.MountedCenter + new Vector2(player.width * player.direction, 0);
        Projectile.rotation = 0;

        Projectile.velocity = Vector2.Zero;

        if (!dir.HasNaNs())
        {
            player.ChangeDir(Math.Sign(dir.X));
            Projectile.spriteDirection = player.direction;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.Gold, Projectile.Opacity * 0.5f, Projectile.scale * 1.5f);
        return true;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
