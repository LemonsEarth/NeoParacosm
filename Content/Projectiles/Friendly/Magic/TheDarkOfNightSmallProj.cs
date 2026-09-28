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
        Projectile.timeLeft = 240;
        Projectile.penetrate = 2;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 40;
    }

    float actualSpeed = 0f;
    public override void AI()
    {
        ParticleSystem.SpawnParticle(
            ParticleID.Gas,
            Projectile.RandomPos(),
            Vector2.Zero,
            Color.Black,
            scale: Main.rand.NextFloat(0.25f, 0.6f)
            );

        for (int i = 0; i < 2; i++)
        {
            ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                ParticleID.Glowy,
                Projectile.RandomPos(),
                Vector2.Zero,
                Main.rand.NextFromList(Color.DarkSlateBlue, Color.CornflowerBlue, Color.LightBlue, Color.SlateBlue),
                scale: Main.rand.NextFloat(0.25f, 0.6f),
                data0: 45,
                data1: 10,
                data2: 20,
                data3: 0.95f
                );
        }

        if (AITimer <= 0)
        {
            closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, HomingRange);
            if (closestNPC != null)
            {
                if (actualSpeed < Speed) actualSpeed += 0.1f;
                Projectile.TurningMoveToPos(closestNPC.Center, 15, actualSpeed);
            }
        }
        else
        {
            Projectile.velocity *= 0.98f;
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
