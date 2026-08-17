namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class EnhancedBullet : ModProjectile
{
    int AITimer;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.timeLeft = 600;

        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = 1;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        return true;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
