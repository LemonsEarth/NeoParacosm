using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class LeafProjectile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float speed => ref Projectile.ai[1];
    ref float accel => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.timeLeft = 240;
        Projectile.penetrate = 1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GrassBlades).noGravity = true;

        if (AITimer == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * speed;
            }
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item39 with { PitchRange = (-0.1f, 0.1f) }, Projectile.Center);
        }
        else if (AITimer > 0)
        {
            Projectile.velocity *= 0.95f;
            Projectile.tileCollide = false;
        }
        else
        {
            Projectile.velocity *= accel;
            Projectile.tileCollide = true;
        }

        if (Projectile.velocity.Length() > 2f)
        {
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation() + MathHelper.PiOver2, 1 / 2f);
        }
        else
        {
            Projectile.rotation += 12f;
        }

        AITimer--;
    }

    public override void OnKill(int timeLeft)
    {
        //SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        SoundEngine.PlaySound(SoundID.Grass with { PitchRange = (-0.1f, 0.1f) }, Projectile.Center);
        Dust.NewDust(Projectile.Center, 2, 2, DustID.GrassBlades, Projectile.velocity.X / 2, Projectile.velocity.Y / 2);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(lightColor, 0.5f);

        return true;
    }
}
