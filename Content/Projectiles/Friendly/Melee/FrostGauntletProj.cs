namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class FrostGauntletProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    bool wasHit = false;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 26;
        Projectile.height = 26;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 5;
        Projectile.timeLeft = 60;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    int sign = 1;
    public override void AI()
    {
        if (AITimer == 0)
        {
            sign = LemonUtils.Sign(Projectile.velocity.X, 1);
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * sign;
        if (sign == -1)
        {
            Projectile.rotation += MathHelper.Pi;
        }
        Projectile.spriteDirection = sign;

        Projectile.velocity *= 0.9f;
        Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 60f);

        //Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Stone).noGravity = true;
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawAfterimages(Projectile, lightColor, 0.5f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        return true;
    }
}

public class FrostGauntletGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public int HitCount { get; set; } = 0;

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.type == ProjectileType<FrostGauntletProj>())
        {
            HitCount++;
        }

        if (HitCount >= 15)
        {
            for (int i = 0; i < 8; i++)
            {
                Projectile.NewProjectileDirect(
                    projectile.GetSource_FromThis(),
                    npc.RandomPos(),
                    Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4f, 8f),
                    ProjectileID.FrostBoltSword,
                    hit.Damage,
                    2f,
                    projectile.owner
                    );
            }
            HitCount = 0;
        }
    }
}
