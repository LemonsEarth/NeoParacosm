

using NeoParacosm.Common.Utils;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class AshenBolt : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float speed => ref Projectile.ai[1];
    ref float doBurst => ref Projectile.ai[2];
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
        Projectile.penetrate = 2;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void AI()
    {
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeStainedGlass, Scale: 1.5f).noGravity = true;
        }
        if (AITimer <= 0)
        {
            if (doBurst == 0)
            {
                closestNPC = LemonUtils.GetClosestNPC(Projectile, 800);
                if (closestNPC != null)
                {
                    speed += 0.3f;
                    Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                }
            }
        }
        else
        {
            Projectile.velocity /= 1.05f;
        }

        if (AITimer == 120)
        {
            if (doBurst == 1)
            {
                Projectile.penetrate = 1;
            }
            else
            {
                Projectile.penetrate = 2;
            }
        }

        AITimer--;
    }

    public override void OnKill(int timeLeft)
    {
        if (doBurst == 1)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 3; i++)
                {
                    LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(8, 12), Type, ai0: 120, ai2: 0);
                }
            }
        }
        //SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 7f, color: Color.Orange);
    }
}
