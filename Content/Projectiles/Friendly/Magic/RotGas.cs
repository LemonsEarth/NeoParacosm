using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class RotGas : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 240;
        Projectile.penetrate = 6;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        
    }

    public override void AI()
    {
        if (AITimer % 10 == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.RandomPos(), Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(1, 2), ModContent.GoreType<RedSmokeGore>(), Main.rand.NextFloat(0.8f, 1.2f));
            }
        }
        
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        
    }
}
