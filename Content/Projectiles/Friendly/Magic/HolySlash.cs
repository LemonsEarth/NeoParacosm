namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class HolySlash : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float DecelerationRate => ref Projectile.ai[1];
    ref float TimeLeft => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.timeLeft = 240;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemTopaz).noGravity = true;

        Player player = Projectile.GetOwner();
        Projectile.velocity *= DecelerationRate;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }

        Projectile.Opacity = 1 - (AITimer / TimeLeft);
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(lightColor, 0.5f);

        return true;
    }
}
