using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class SplittingCrystal : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float SplitCount => ref Projectile.ai[1];
    ref float TimeLeft => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.frame = Main.rand.Next(0, 3);
        }

        if (AITimer > TimeLeft)
        {
            Split();
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        //Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Stone).noGravity = true;
        AITimer++;
    }

    void Split()
    {
        if (SplitCount > 0)
        {
            SplitCount--;
            for (int i = -1; i <= 1; i++)
            {
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.Center,
                    Projectile.velocity.RotatedBy(i * MathHelper.Pi / 8) * 1.2f,
                    Type,
                    ai1: SplitCount,
                    ai2: TimeLeft * 0.8f
                    );
            }
        }
        Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 5, DustID.PinkCrystalShard, scale: 1.5f);
        SoundEngine.PlaySound(SoundID.Item27 with { MaxInstances = 1, Volume = 0.5f, PitchRange = (0.1f, 0.6f) });
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        //LemonUtils.DrawAfterimages(Projectile, lightColor, 0.1f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        return true;
    }
}
