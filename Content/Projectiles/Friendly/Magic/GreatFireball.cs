using NeoParacosm.Common.Utils;
using Terraria;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class GreatFireball : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.friendly = true;
        Projectile.timeLeft = 480;
        Projectile.penetrate = 1;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.extraUpdates = 2;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 180);
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
        }
        if (AITimer < 20)
        {
            Projectile.tileCollide = false;
        }
        else
        {
            Projectile.tileCollide = true;
        }
        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.OrangeStainedGlass, Scale: 3f, newColor: Color.OrangeRed).noGravity = true;
        Dust.NewDustDirect(Projectile.RandomPos( - 24, - 24), 2, 2, DustID.GemTopaz, Scale: 2f, newColor:Color.Yellow).noGravity = true;


        Projectile.velocity.Y += 0.1f;

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<GreatFireballExplosion>());
    }
}
