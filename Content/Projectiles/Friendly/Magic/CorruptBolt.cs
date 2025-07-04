using NeoParacosm.Common.Utils;
using Terraria.Audio;
namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class CorruptBolt : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float angle => ref Projectile.ai[1];

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
        Projectile.timeLeft = 90;
        Projectile.penetrate = 2;
        Projectile.Opacity = 0f;
        Projectile.tileCollide = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, Projectile.Center);
        }

        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Scale: 1.5f, newColor: Color.Purple).noGravity = true;
        }

        Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * MathHelper.ToRadians(angle);

        AITimer--;
    }

    public override void OnKill(int timeLeft)
    {
        
        //SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.Corruption, 4f, color: Color.Purple);
    }
}
