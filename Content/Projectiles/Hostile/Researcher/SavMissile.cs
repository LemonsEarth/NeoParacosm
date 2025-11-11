using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Researcher;

public class SavMissile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float TrackingTime => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.scale = 1f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return true;
    }

    Vector2 savedVelocity = Vector2.Zero;
    int soundTimer = 30;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedVelocity = Projectile.velocity;
            SoundEngine.PlaySound(SoundID.Item61 with { Volume = 0.75f}, Projectile.Center);

        }

        if (AITimer < TrackingTime)
        {
            if (AITimer % 45 == 0)
            {
                SoundEngine.PlaySound(SoundID.Zombie67 with { Pitch = 2f, PitchVariance = 0, Volume = 0.25f, MaxInstances = 1 }, Projectile.Center);
            }
            Player player = LemonUtils.GetClosestPlayer(Projectile.Center);
            if (player != null && player.Alive())
            {
                Projectile.velocity = Projectile.DirectionTo(player.Center) * savedVelocity.Length();
            }
        }
        else
        {
            if (AITimer % soundTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Zombie67 with { Pitch = 2f, PitchVariance = 0, Volume = 0.25f, MaxInstances = 1 }, Projectile.Center);
            }
            if (soundTimer > 5) soundTimer -= 5;
            Projectile.velocity *= 1.02f;
        }

        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);

        var dust = Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Electric);
        dust.noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        LemonUtils.StandardAnimation(Projectile, 6, 3);
        Projectile.velocity *= 1.03f;
        AITimer++;
    }

    public override void PostDraw(Color lightColor)
    {
        float percent = (180f - Projectile.timeLeft) / 180f;
        LemonUtils.DrawGlow(Projectile.Center, Color.LightBlue, percent, 1 + percent);
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemSapphire, 2f);
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<ElectroGasHostile>(), Projectile.damage / 2);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        
    }
}
