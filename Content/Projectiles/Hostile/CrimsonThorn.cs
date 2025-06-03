using NeoParacosm.Common.Utils;

namespace NeoParacosm.Content.Projectiles.Hostile;

public class CrimsonThorn: ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.scale = 1f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 0);

        if (AITimer > 20)
        {
            Projectile.velocity.Y += 1;
        }
        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson);
        dust.noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.Bleeding, 600);

        if (Main.rand.NextBool(3))
        {
            target.AddBuff(BuffID.Ichor, 300);
        }
    }
}
