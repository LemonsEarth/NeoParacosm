using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class BloodBloomProj : ModProjectile
{
    int AITimer = 0;
    ref float SlowdownRate => ref Projectile.ai[0];
    ref float TimeLeft => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.scale = 0.75f;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    bool damageSet = false;
    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.rotation = Main.rand.NextFloat(0, 6.28f);
        }

        if (AITimer == TimeLeft - 3)
        {
            Projectile.damage = Projectile.originalDamage * 2;
            Projectile.Opacity = 0f;
            Projectile.Resize(64, 64);
            if (Main.myPlayer == Projectile.owner)
            {
                LemonUtils.QuickPulse(
                    Projectile,
                    Projectile.Center,
                    2f,
                    1f,
                    10f,
                    Color.Red
                    );
            }
            SoundEngine.PlaySound(SoundID.Item84 with { PitchRange = (0.7f, 0.9f), MaxInstances = 0 }, Projectile.Center);
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
            return;
        }

        Projectile.velocity *= SlowdownRate;

        //Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Stone).noGravity = true;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!damageSet && AITimer < TimeLeft - 3)
        {
            AITimer = (int)(TimeLeft - 3);
        }
        damageSet = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.Red, Projectile.Opacity, Projectile.scale);
        //LemonUtils.DrawAfterimages(Projectile, lightColor, 0.1f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        Projectile.DrawProjectile(lightColor);
        return false;
    }
}
