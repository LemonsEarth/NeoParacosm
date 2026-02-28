using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Ice.EmperorColdsteel;

public class IceCometSmallHostile : PrimProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float WaitTime => ref Projectile.ai[1];
    ref float PosY => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 9999;
    }

    bool exploded = false;
    Vector2 savedVelocity;
    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item117 with { PitchRange = (0.4f, 0.6f) }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item29 with { PitchRange = (0.4f, 0.6f), Volume = 0.5f }, Projectile.Center);
            savedVelocity = Projectile.velocity;
        }

        Projectile.rotation = MathHelper.ToRadians(AITimer * 3);

        if (AITimer < WaitTime)
        {
            Projectile.velocity = Vector2.Zero;
            Vector2 savedVelNormalized = savedVelocity.SafeNormalize(Vector2.Zero);
            Vector2 topPos = Projectile.Center + savedVelNormalized.RotatedBy(-MathHelper.PiOver2) * (Projectile.width * 0.5f);
            Vector2 botPos = Projectile.Center + savedVelNormalized.RotatedBy(MathHelper.PiOver2) * (Projectile.width * 0.5f);
            float count = AITimer / WaitTime;
            Projectile.Opacity = count;
            for (int i = 0; i < 8 * count; i++)
            {
                Dust topDust = Dust.NewDustDirect(topPos + savedVelNormalized * i * 64, 2, 2, DustID.IceTorch, Scale: Main.rand.NextFloat(1.5f, 2.5f));
                topDust.noGravity = true;
                topDust.velocity = savedVelNormalized * 5;
                Dust botDust = Dust.NewDustDirect(botPos + savedVelNormalized * i * 64, 2, 2, DustID.IceTorch, Scale: Main.rand.NextFloat(1.5f, 2.5f));
                botDust.noGravity = true;
                botDust.velocity = savedVelNormalized * 5;
            }
        }
        else if (AITimer == WaitTime)
        {
            Projectile.velocity = savedVelocity;
        }

        if (AITimer > TimeLeft && !exploded)
        {
            exploded = true;
            Projectile.Resize(132 * 3, 132 * 3);
            Projectile.timeLeft = 3;
        }
        Dust.NewDustDirect(Projectile.RandomPos(), 1, 1, DustID.IceTorch, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustBurst(60, Projectile.Center, DustID.GemSapphire, 40, 40, 3f, 5f);
        if (LemonUtils.NotClient())
        {
            LemonUtils.QuickPulse(Projectile, Projectile.Center, 3, 6, 10, Color.Cyan);
        }
        SoundEngine.PlaySound(SoundID.Item14 with { PitchRange = (-0.5f, 0.5f) }, Projectile.Center);
        SoundEngine.PlaySound(SoundID.Item89 with { PitchRange = (-0.5f, 0.5f) }, Projectile.Center);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn2, 120);
        Projectile.damage -= (int)(0.2f * Projectile.originalDamage);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (exploded) return true;
        /*if (AITimer < WaitTime)
        {
            LemonUtils.DrawLaser(Projectile.Center, Projectile.Center + savedVelocity.SafeNormalize(Vector2.Zero) * 1000, 2, Color.Cyan);
        }*/
        PrimHelper.DrawBasicProjectilePrimTrailRectangular(Projectile, Color.Cyan * 0.5f, Color.Transparent, BasicEffect);
        return true;
    }
}
