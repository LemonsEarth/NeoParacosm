using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil;

public class ArenaBoundLostSoul : ModProjectile
{
    int AITimer = 0;
    ref float Duration => ref Projectile.ai[0];
    ref float ChaseSpeed => ref Projectile.ai[1];
    ref float Range => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 28;
        Projectile.height = 28;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 900;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 0.5f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    Player player = null;
    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemRuby, 3f);
        }
        if (player == null || !player.Alive())
        {
            player = LemonUtils.GetClosestPlayer(Projectile.Center, Range);
        }
        if (player != null && player.Alive())
        {
            Projectile.velocity = Projectile.DirectionTo(player.Center) * ChaseSpeed;
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 1);

        for (float i = 0; i < 1 + Projectile.velocity.Length() / 4f; i++)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(-8, -8), DustID.Ash, Scale: Main.rand.NextFloat(2, 3), newColor: Color.Black).noGravity = true;
            Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.Crimson, Vector2.Zero, Scale: 1.2f).noGravity = true;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.rotation > MathHelper.PiOver2 || Projectile.rotation < -MathHelper.PiOver2)
        {
            Projectile.spriteDirection = -1;
            Projectile.rotation += MathHelper.Pi;
        }
        else
        {
            Projectile.spriteDirection = 1;
        }

        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        Projectile.StandardAnimation(18, 2);

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.DarkRed, Projectile.Opacity, Projectile.scale * 2);
        return true;
    }
}
