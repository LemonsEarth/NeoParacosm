using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;

public class ShadowBolt : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
        ProjectileID.Sets.SentryShot[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = 3;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 180;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void AI()
    {
        Projectile.rotation = MathHelper.ToRadians(AITimer);

        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.Shadowflame, Projectile.velocity.X, Projectile.velocity.Y);
        dust.noGravity = true;
        Lighting.AddLight(Projectile.Center, 2, 0, 2);
        ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
               ParticleID.Glowy,
               Projectile.RandomPos(8, 8),
               Main.rand.NextVector2Circular(2, 2),
               Main.rand.NextFromList(new Color(60, 0, 120)),
               0f,
               scale: 1f,
               data0: 30,
               data1: 5,
               data2: 15,
               data3: 0.93f);
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 16, 5, DustID.GemAmethyst);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.ShadowFlame, 240);
    }
}
