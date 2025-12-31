using NeoParacosm.Common.Utils;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil;

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
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 3, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        }

        if (AITimer <= 2)
        {
            Projectile.Opacity = AITimer / 2f;
        }

        if (Duration - AITimer < 5)
        {
            Projectile.Opacity -= 1 / 5f;
        }

        if (AITimer > Duration * 0.5f)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(TurningAngle);
        }

        Lighting.AddLight(Projectile.Center, 0, 5, 0);

        if (AITimer % 10 == 0)
        {
            Dust.NewDustDirect(Projectile.RandomPos(0, 0), 2, 2, DustID.IchorTorch, Scale: Main.rand.NextFloat(2.5f, 4f)).noGravity = true;
        }

        Projectile.velocity *= SlowDownRate;

        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        int frameDuration = (int)(Duration / 6);
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
        target.AddBuff(BuffID.Ichor, 360);
    }
}
