using NeoParacosm.Content.Projectiles.Friendly.Magic;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class ThunderClapProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

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

public class ThunderClapGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public int HitCount { get; set; } = 0;

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.type == ProjectileType<ThunderClapProj>())
        {
            HitCount++;
        }

        if (HitCount >= 15)
        {
            int npcCount = 0;
            foreach (var otherNPC in Main.ActiveNPCs)
            {
                if (npcCount >= 8)
                {
                    break;
                }
                Vector2 toOther = otherNPC.Center - npc.Center;
                float distance = toOther.Length();
                if (distance < 500)
                {
                    Vector2 dir = toOther.SafeNormalize(Vector2.Zero);
                    LemonUtils.QuickProj(
                        projectile,
                        npc.Center,
                        dir,
                        ProjectileType<HolyLightningFriendly>(),
                        ai0: 0,
                        ai1: distance
                    );
                    npcCount++;
                }
            }
            HitCount = 0;
        }
    }
}
