using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;

public class GiantShiverthorn : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float AttackTimer => ref Projectile.ai[1];
    NPC closestEnemy;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 58;
        Projectile.height = 50;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.tileCollide = true;
        Projectile.timeLeft = Projectile.SentryLifeTime;
        Projectile.friendly = false;
        Projectile.sentry = true;
    }

    public override void AI()
    {
        closestEnemy = LemonUtils.GetClosestNPC(Projectile.Center, 1000);

        Projectile.velocity.Y = 10f;
        if (closestEnemy != null)
        {
            if (AttackTimer == 90)
            {
                if (LemonUtils.NotClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.Center.DirectionTo(closestEnemy.Center) * 5, ProjectileType<IceProjectile>(), Projectile.damage, 1f, Projectile.owner);
                }
                SoundEngine.PlaySound(SoundID.Item28 with { Volume = 0.5f, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
                AttackTimer = 0;
            }
            AttackTimer++;
        }
        else
        {
            AttackTimer = 0;
        }

        Projectile.frameCounter++;
        if (Projectile.frameCounter == 20)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 2)
            {
                Projectile.frame = 0;
            }
        }
        Lighting.AddLight(Projectile.Center, 0, 0, 5);
        AITimer++;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity.Y = 0;
        return false;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }
}
