using NeoParacosm.Content.Buffs.Debuffs;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class GiantSpikedBallProj : ModProjectile
{
    int AITimer = 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.SpikyBall);
        Projectile.width = 38;
        Projectile.height = 38;
        Projectile.penetrate = 10;
        Projectile.idStaticNPCHitCooldown = 20;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
        Projectile.timeLeft = 1200;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {

        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, 180);
    }

    public override void AI()
    {
        AITimer++;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustBurst(16, Projectile.Center, DustID.Torch, 5, 5, 3f, 3.4f);
        //LemonUtils.DustBurst(7, Projectile.Center, DustID.GemTopaz, 5, 5, 1f, 1.2f);
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return true;
    }
}
