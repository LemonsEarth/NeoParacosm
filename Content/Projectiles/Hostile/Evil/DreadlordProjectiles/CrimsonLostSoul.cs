namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class CrimsonLostSoul : ModProjectile
{
    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];
    ref float Duration => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 28;
        Projectile.height = 28;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 0.5f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    float maxSpeed = 0;
    float currentSpeed = 0;
    float speedAddValue = 0.05f;
    Player player = null;
    public override void AI()
    {
        if (AITimer == 0)
        {
            maxSpeed = Projectile.velocity.Length() * 2;
            currentSpeed = Projectile.velocity.Length();
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemRuby, 3f);
        }
        if (player == null || !player.Alive())
        {
            player = LemonUtils.GetClosestPlayer(Projectile.Center, 1000);
        }
        if (player != null && player.Alive() && AITimer > WaitTime)
        {
            if (currentSpeed < maxSpeed) currentSpeed += speedAddValue;
            float angleDifference = MathHelper.WrapAngle(Projectile.Center.DirectionTo(player.Center).ToRotation() - Projectile.velocity.ToRotation());
            Projectile.velocity = Projectile.velocity.RotatedBy(angleDifference / 20f).SafeNormalize(Vector2.Zero) * currentSpeed;
            //Projectile.velocity += Projectile.Center.DirectionTo(player.Center);
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 1);


        Dust.NewDustPerfect(Projectile.RandomPos(-8, -8), DustID.Ash, Scale: Main.rand.NextFloat(2, 3), newColor: Color.Black).noGravity = true;
        Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.Crimson, Vector2.Zero, Scale: 1.2f).noGravity = true;


        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.rotation > MathHelper.PiOver2 || Projectile.rotation < -MathHelper.PiOver2)
        {
            Projectile.spriteDirection = -1;
            Projectile.rotation += MathHelper.Pi;
        }
        else
        {
            Projectile.spriteDirection = 1;
        }

        Projectile.StandardAnimation(18, 2);

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.DarkRed, Projectile.Opacity, Projectile.scale);
        return true;
    }
}

