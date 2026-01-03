namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class NatureBolt : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float speed => ref Projectile.ai[1];
    NPC closestNPC;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 240;
        Projectile.penetrate = 2;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra, newColor: Color.Lime, Scale: 1.5f).noGravity = true;
        }
        if (AITimer <= 0)
        {
            closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, 400);
            if (closestNPC != null)
            {
                speed += 0.3f;
                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
            }
        }
        else
        {
            Projectile.velocity /= 1.05f;
        }
        AITimer--;
    }

    public override void OnKill(int timeLeft)
    {
        //SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 7f, color: Color.Lime);
    }
}
