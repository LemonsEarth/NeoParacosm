using NeoParacosm.Content.Buffs.Debuffs;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class GlowingMushroomProj : ModProjectile
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
        Projectile.width = 28;
        Projectile.height = 28;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.scale = 1f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        Lighting.AddLight(Projectile.Center, 0, 0, 1);

        Projectile.velocity.Y += 0.1f;

        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GlowingMushroom);
        dust.noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 18);
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffType<ShroomedDebuff>(), 300);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 2, DustID.GlowingMushroom);
    }
}
