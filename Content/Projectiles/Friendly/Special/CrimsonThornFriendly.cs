namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class CrimsonThornFriendly : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
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
        Projectile.timeLeft = 300;
        Projectile.scale = 0.8f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        Lighting.AddLight(Projectile.Center, 1, 1, 0);

        if (AITimer > 20)
        {
            Projectile.velocity.Y += 1;
        }
        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson);
        dust.noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }
}
