namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class FrostShurikenProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    bool wasHit = false;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 10;
        Projectile.timeLeft = 600;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    int sign = 1;
    public override void AI()
    {
        if (AITimer == 0)
        {
            sign = LemonUtils.Sign(Projectile.velocity.X, 1);
        }

        Projectile.rotation = MathHelper.ToRadians(AITimer * 20 * sign);
        Projectile.spriteDirection = sign;
        if (wasHit)
        {
            Projectile.velocity = Vector2.UnitY.RotatedByRandom(6.28f) * 5f;
        }


        //Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Stone).noGravity = true;
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        wasHit = true;
        Projectile.ArmorPenetration += 1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawAfterimages(Projectile, lightColor, 0.5f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        return true;
    }
}
