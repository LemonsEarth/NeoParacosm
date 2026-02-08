
namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class AssassinsBackupProj : ModProjectile
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
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ash);
        dust.noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(lightColor, 0.5f);
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (int i = 0; i < 8; i++)
        {
            Dust.NewDustDirect(Projectile.position, 2, 2, DustID.Ash, Projectile.velocity.X / 2, Projectile.velocity.Y / 2).noGravity = true;
        }

        if (Main.rand.NextBool(4))
        {
            int healAmount = 2;
            if (hit.Crit)
            {
                healAmount = 10;
            }
            Projectile.GetOwner().Heal(healAmount);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }
}
