using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class TheDarkOfNightSmallProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float Speed => ref Projectile.ai[1];
    ref float HomingRange => ref Projectile.ai[2];
    NPC closestNPC;

    public override string Texture => ParacosmTextures.Empty100TexPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
        Projectile.penetrate = 6;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 40;
        Projectile.extraUpdates = 3;
    }

    float actualSpeed = 0f;
    float turningDenominator = 30f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            turningDenominator = 30f;
        }
        ParticleSystem.SpawnParticle(
            ParticleID.Gas,
            Projectile.RandomPos(),
            Projectile.velocity,
            Color.Black,
            scale: Main.rand.NextFloat(0.4f, 0.8f)
            );


        ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
            ParticleID.Glowy,
            Projectile.Center + Main.rand.NextVector2Circular(2, 2),
            Projectile.velocity,
            Main.rand.NextFromList(Color.DarkSlateBlue, Color.CornflowerBlue),
            scale: Main.rand.NextFloat(0.4f, 0.7f),
            data0: 30,
            data1: 10,
            data2: 15,
            data3: 0.99f
            );


        if (AITimer <= 0)
        {
            closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, HomingRange);
            if (closestNPC != null)
            {
                if (actualSpeed < Speed) actualSpeed += 0.1f;
                Projectile.TurningMoveToPos(closestNPC.Center, turningDenominator, actualSpeed);
                if (turningDenominator > 5)
                {
                    turningDenominator -= 0.02f;
                }
            }
        }
        else
        {
            Projectile.velocity *= 0.999f;
        }
        AITimer--;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnKill(int timeLeft)
    {

    }
}
