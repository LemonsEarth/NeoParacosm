namespace NeoParacosm.Content.Projectiles.Hostile.Death;

public class WallP : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 0f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return null;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
           
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        
        return false;
    }
}
