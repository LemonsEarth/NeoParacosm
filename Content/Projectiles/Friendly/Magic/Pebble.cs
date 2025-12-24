
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class Pebble : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    static BasicEffect BasicEffect;

    public override void Load()
    {
        if (Main.dedServ) return;
        Main.RunOnMainThread(() =>
        {
            BasicEffect = new BasicEffect(PrimHelper.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };
        });
    }

    public override void Unload()
    {
        if (Main.dedServ) return;
        Main.RunOnMainThread(() =>
        {
            BasicEffect?.Dispose();
            BasicEffect = null;
        });
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 2;
        Projectile.timeLeft = 15;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.frame = Main.rand.Next(0, 3);
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Stone).noGravity = true;
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
       
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawAfterimages(Projectile, lightColor, 0.1f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        return true;
    }
}
