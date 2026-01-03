using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class LamentOfTheLateProj : ModProjectile
{
    int AITimer = 0;
    ref float ProjType => ref Projectile.ai[0];
    ref float WaitTime => ref Projectile.ai[1];
    ref float ProjInterval => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 200;
        Projectile.height = 200;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        if (AITimer > WaitTime)
        {
            if (AITimer % ProjInterval == 0)
            {
                SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
                if (LemonUtils.NotClient())
                {
                    Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-Projectile.width / 2, Projectile.width / 2), -600);
                    LemonUtils.QuickProj(Projectile, pos, Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 12, MathHelper.Pi / 12)) * 16, (int)ProjType).extraUpdates = 1;
                }
            }
        }

        Projectile.velocity = Vector2.Zero;

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }
}
