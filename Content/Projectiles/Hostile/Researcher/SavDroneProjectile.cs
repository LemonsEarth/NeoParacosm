using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Researcher;

public class SavDroneProjectile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 300;
        Projectile.scale = 1f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item92 with { PitchRange = (0.9f, 1f), Volume = 0.5f }, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);

        var dust = Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Electric);
        dust.noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        LemonUtils.StandardAnimation(Projectile, 6, 4);
        Projectile.velocity *= 1.03f;
        AITimer++;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        
    }
}
