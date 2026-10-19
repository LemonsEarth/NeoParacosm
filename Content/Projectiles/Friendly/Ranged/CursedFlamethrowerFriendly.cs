using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class CursedFlamethrowerFriendly : ModProjectile
{
    int AITimer = 0;
    ref float Duration => ref Projectile.ai[0];
    ref float SlowDownRate => ref Projectile.ai[1];
    ref float TurningAngle => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults()
    {
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.Opacity = Main.rand.NextFloat(0.5f, 1f);
            Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 10, PitchRange = (-0.2f, 0.2f), Volume = 0.75f }, Projectile.Center);
        }

        if (Duration - AITimer < 5)
        {
            Projectile.Opacity -= 1 / 5f;
        }

        if (AITimer > Duration * 0.5f)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(TurningAngle);
        }

        Lighting.AddLight(Projectile.Center, 0, 5, 0);

        /*if (Main.rand.NextBool(5))
        {
            //Dust.NewDustPerfect(Projectile.RandomPos(0, 0), DustID.CursedTorch, Projectile.velocity * 0.5f, Scale: Main.rand.NextFloat(2f, 3f)).noGravity = true;
            ParticleSystem.SpawnParticle(ParticleID.Gas, Projectile.RandomPos(), Projectile.velocity * 0.2f, Color.LimeGreen);
        }*/

        for (int i = 0; i < 10; i++)
        {
            ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                ParticleID.Glowy,
                Projectile.RandomPos(-8, -8),
                Vector2.UnitY.RotatedBy(Main.rand.NextRotation()),
                Main.rand.NextFromList(Color.Green, Color.Lime, Color.LightGreen) * 0.5f,
                1f,
                1f,
                data0: 30,
                data1: 5f,
                data2: 20f,
                data3: 0.98f
                );
        }
        Projectile.velocity *= SlowDownRate;

        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        int frameDuration = (int)(Duration / 6);
        if (Projectile.frame < 5) // safeguard so it never loops
        {
            Projectile.StandardAnimation(frameDuration, 6);
        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.CursedInferno, 300);
    }
}
