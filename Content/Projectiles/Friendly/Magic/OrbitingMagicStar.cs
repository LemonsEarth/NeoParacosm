using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class OrbitingMagicStar : PrimProjectile
{
    int AITimer = 0;
    ref float Duration => ref Projectile.ai[0];
    ref float PosOffset => ref Projectile.ai[1];
    ref float RotOffset => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 6666;
        Projectile.penetrate = 10;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        Projectile.timeLeft = 2;
        Player player = Projectile.GetOwner();
        Projectile.rotation = AITimer * 6;
        Projectile.Center = player.Center + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(AITimer * 6) + RotOffset) * PosOffset;
        ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                ParticleID.Glowy,
                Projectile.RandomPos(2, 2),
                Vector2.UnitY.RotatedBy(Main.rand.NextRotation()),
                Main.rand.NextFromList(Color.CornflowerBlue, Color.Blue, Color.DarkSlateBlue) * 0.8f,
                1f,
                Main.rand.NextFloat(1f, 1.5f),
                data0: 20,
                data1: 5f,
                data2: 10f,
                data3: 0.98f
                );
        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        Lighting.AddLight(Projectile.position, 2, 2, 2);
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemSapphire).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.DarkSlateBlue, 1f, Projectile.scale);
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.LightBlue, Color.Transparent, BasicEffect);
        return true;
    }
}
