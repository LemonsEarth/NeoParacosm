using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class DeathflameBallFriendly : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float speed => ref Projectile.ai[1];
    NPC closestNPC;

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
        Projectile.penetrate = 1;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        for (int i = 0; i < 2; i++)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(-8, -8), DustID.Ash, Scale: Main.rand.NextFloat(1, 2), newColor: Color.Black).noGravity = true;
            Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.GemDiamond, Vector2.Zero, newColor: Color.White, Scale: 1.1f).noGravity = true;
        }
        if (AITimer <= 0)
        {
            closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, 600);
            if (closestNPC != null)
            {
                if (speed < 3)
                {
                    speed += 0.3f;
                }
                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
            }
        }
        else
        {
            Projectile.velocity *= 0.97f;
        }
        AITimer--;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffType<DeathflameDebuff>(), 180);
    }

    public override void OnKill(int timeLeft)
    {
        //SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        for (int i = 0; i < 4; i++)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(-8, -8), DustID.Ash, Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(2, 4), Scale: Main.rand.NextFloat(2, 3), newColor: Color.Black).noGravity = true;
            Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.GemDiamond, Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(2, 4), newColor: Color.White, Scale: 1.2f).noGravity = true;
        }
    }
}
