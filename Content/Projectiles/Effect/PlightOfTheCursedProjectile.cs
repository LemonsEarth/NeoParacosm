using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Effect;

public class PlightOfTheCursedProjectile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float Volume => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 86;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.frame = Main.rand.Next(1, 4);
            SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f, Volume = Volume, MaxInstances = 0 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f, Volume = Volume, MaxInstances = 0 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f, Volume = Volume, MaxInstances = 0 }, Projectile.Center);
        }
        Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0, AITimer / 120f);
        Projectile.velocity = -Vector2.UnitY * 2;
        AITimer++;
    }
}
