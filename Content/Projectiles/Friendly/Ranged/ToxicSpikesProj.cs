using NeoParacosm.Content.Buffs.Debuffs;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class ToxicSpikesProj : ModProjectile
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
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
        AIType = ProjectileID.SpikyBall;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {

        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        ToxicDebuff.AddToNPC(target, 45);
    }

    float rotValue = 0;
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

    }

    public override bool PreDraw(ref Color lightColor)
    {

        return true;
    }
}
