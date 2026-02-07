namespace NeoParacosm.Content.Projectiles.Hostile.Evil;

public class CursedDecayFire : ModProjectile
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
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 420;
        Projectile.scale = 1f;
        Projectile.Opacity = 0f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        if (Projectile.velocity.Y == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomPos = Projectile.Bottom + new Vector2(Main.rand.NextFloat(-Projectile.width, Projectile.width), 0);
                Dust.NewDustPerfect(randomPos, DustID.CursedTorch, -Vector2.UnitY * 3, Scale: 2).noGravity = true;
            }
            Projectile.velocity.X = 0;
            Projectile.width = 32;
        }
        else
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.CursedTorch, 0, 0, Scale: 2f).noGravity = true;
            Projectile.width = 16;
        }

        Lighting.AddLight(Projectile.Center, 0, 1, 0);

        Projectile.velocity.Y += 0.1f;

        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.CursedInferno, 300);
    }
}
