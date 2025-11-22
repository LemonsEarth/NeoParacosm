using NeoParacosm.Common.Utils;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class GreatFireballExplosion : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.timeLeft = 45;
        Projectile.penetrate = -1;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
        Projectile.extraUpdates = 2;
        Projectile.tileCollide = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 180);
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.6f}, Projectile.Center);
        }
        for (int i = 0; i < 5; i++)
        {
            Vector2 randVector = Main.rand.NextVector2Circular(10, 10);
            Vector2 randVector2 = Main.rand.NextVector2Circular(3, 3);
            Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustID.OrangeStainedGlass, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustID.GemTopaz, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        }

        Projectile.velocity = Vector2.Zero;

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        
    }
}
