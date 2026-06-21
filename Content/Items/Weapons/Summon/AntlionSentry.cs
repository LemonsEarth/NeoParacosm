namespace NeoParacosm.Content.Items.Weapons.Summon;

public class AntlionSentry : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    NPC closestEnemy;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 40;
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
        if (Main.myPlayer == Projectile.owner && AITimer % 60 == 0 && closestEnemy != null)
        {
            Vector2 spawnPos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, 20));
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, (closestEnemy.Center - spawnPos).SafeNormalize(Vector2.Zero) * 10, ProjectileID.SandBallGun, Projectile.damage, 10f, Projectile.owner);
            Projectile.netUpdate = true;
        }

        int frameDur = 20;
        Projectile.frameCounter++;
        if (Projectile.frameCounter == frameDur)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame == 2)
            {
                Projectile.frame = 0;
            }
        }
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
