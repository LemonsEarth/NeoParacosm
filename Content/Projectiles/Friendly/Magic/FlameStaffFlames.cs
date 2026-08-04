using NeoParacosm.Core.Systems.Assets;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class FlameStaffFlames : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float TimeLeft => ref Projectile.ai[1];

    public override string Texture => ParacosmTextures.Empty100TexPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 5;
        Projectile.timeLeft = 600;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 10;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }

        Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Torch, Scale: 2f).noGravity = true;
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, 120);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //LemonUtils.DrawAfterimages(Projectile, lightColor, 0.1f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        return true;
    }
}
