using NeoParacosm.Common.Utils;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil;

public class CursedGeyser : ModProjectile
{
    int AITimer = 0;
    Vector2 savedVelocity = Vector2.Zero;
    ref float GeyserDuration => ref Projectile.ai[0];
    ref float RandomSpeedOffset => ref Projectile.ai[1];
    ref float HorizontalSpeed => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 68;
        Projectile.height = 62;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.Opacity = 0f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            savedVelocity = Projectile.velocity;
            Projectile.rotation = savedVelocity.ToRotation() + MathHelper.PiOver2;
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(Projectile.RandomPos(-16, -16), 2, 2, DustID.CursedTorch, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
                Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Corruption, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 3, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        }
        Projectile.velocity = new Vector2(HorizontalSpeed, 0);

        if (AITimer <= 5)
        {
            Projectile.Opacity = AITimer / 5f;
        }

        if (AITimer > GeyserDuration)
        {
            Projectile.Kill();
        }

        if (LemonUtils.NotClient() && AITimer % 6 == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.Center,
                    savedVelocity.SafeNormalize(Vector2.Zero)
                        .RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 64, MathHelper.Pi / 64)) 
                        * Main.rand.NextFloat(savedVelocity.Length() - RandomSpeedOffset, savedVelocity.Length() + RandomSpeedOffset),
                    ProjectileType<CursedFlamethrower>(),
                    Projectile.damage,
                    ai0: 30,
                    ai1: 0.97f,
                    ai2: Main.rand.NextFloat(-MathHelper.Pi / 16, MathHelper.Pi / 16)
                    );
            }
        }

        Lighting.AddLight(Projectile.Center, 0, 5, 0);

        Projectile.StandardAnimation(30, 2);

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 5; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(-16, -16), 2, 2, DustID.CursedTorch, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Corruption, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return true;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.CursedInferno, 180);
    }
}
