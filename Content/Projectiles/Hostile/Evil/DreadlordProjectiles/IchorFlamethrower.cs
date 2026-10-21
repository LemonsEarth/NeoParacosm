using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class IchorFlamethrower : ModProjectile
{
    int AITimer = 0;
    ref float Duration => ref Projectile.ai[0];
    ref float SlowDownRate => ref Projectile.ai[1];
    ref float TurningAngle => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults()
    {
        Projectile.width = 120;
        Projectile.height = 120;
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
            Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 16, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchRange = (-0.2f, 0.2f), Volume = 0.75f }, Projectile.Center);
        }

        if (AITimer <= 2)
        {
            Projectile.Opacity = AITimer / 2f;
        }

        if (Duration - AITimer < 5)
        {
            Projectile.Opacity -= 1 / 5f;
        }

        //if (AITimer > Duration * 0.5f)
        //{
            Projectile.velocity = Projectile.velocity.RotatedBy(TurningAngle);
        //}

        Lighting.AddLight(Projectile.Center, 0, 5, 0);


        for (int i = 0; i < 2; i++)
        {
            ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                ParticleID.Glowy,
                Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2),
                Vector2.UnitY.RotatedBy(Main.rand.NextRotation()),
                Main.rand.NextFromList(Color.Yellow, Color.Gold, Color.LightYellow),
                1f,
                Main.rand.NextFloat(1f, 2f),
                data0: 30,
                data1: 5f,
                data2: 20f,
                data3: 0.98f
                );
        }

        Projectile.velocity *= SlowDownRate;

        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        int frameDuration = (int)(Duration / 6f);
        if (Projectile.frame < 5) // safeguard so it never loops
        {
            Projectile.StandardAnimation(frameDuration, 6);
        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(Color.White, 1f);
        LemonUtils.DrawGlow(Projectile.Center, Color.Yellow, Projectile.Opacity, 2f);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.Ichor, 1200);
    }
}
