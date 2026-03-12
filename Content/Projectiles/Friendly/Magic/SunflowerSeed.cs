namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class SunflowerSeed : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 30;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        //Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Stone).noGravity = true;
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        //LemonUtils.DrawAfterimages(Projectile, lightColor, 0.1f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        return true;
    }
}
