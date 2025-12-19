using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil;

public class CrimsonLostSoul : ModProjectile
{
    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];
    ref float PlayerID => ref Projectile.ai[1];
    ref float Duration => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 0.5f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        
    }

    float maxSpeed = 0;
    float currentSpeed = 0;
    float speedAddValue = 0.05f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            maxSpeed = Projectile.velocity.Length() * 2;
            currentSpeed = Projectile.velocity.Length();
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 3f);
        }
        Player player = Main.player[(int)PlayerID];
        if (player.Alive() && AITimer > WaitTime)
        {
            if (currentSpeed < maxSpeed) currentSpeed += speedAddValue;
            float angleDifference = MathHelper.WrapAngle(Projectile.Center.DirectionTo(player.Center).ToRotation() - Projectile.velocity.ToRotation());
            Projectile.velocity = Projectile.velocity.RotatedBy(angleDifference / 20f).SafeNormalize(Vector2.Zero) * currentSpeed;
            //Projectile.velocity += Projectile.Center.DirectionTo(player.Center);
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 1);

        for (float i = 0; i < 1 + Projectile.velocity.Length() / 3f; i++)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(-8, -8), DustID.Ash, Scale: Main.rand.NextFloat(2, 3), newColor: Color.Black).noGravity = true;
            Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.Crimson, Vector2.Zero, Scale: 1.2f).noGravity = true;
        }

        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.DarkRed, Projectile.Opacity, Projectile.scale);
        return false;
    }
}
