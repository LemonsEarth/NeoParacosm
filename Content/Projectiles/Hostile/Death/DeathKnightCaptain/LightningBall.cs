using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;

namespace NeoParacosm.Content.Projectiles.Hostile.Death.DeathKnightCaptain;

public class LightningBall : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;
    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];
    ref float Duration => ref Projectile.ai[1];
    ref float LengthAvg => ref Projectile.ai[2];

    Player closestPlayer;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 1f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        //target.AddBuff(BuffType<DeathflameDebuff>(), 30);
    }

    float speed = 1;
    public override void AI()
    {
        if (AITimer == 0)
        {
            speed = Projectile.velocity.Length();
        }

        if (AITimer > Duration)
        {
            Projectile.Kill();
            return;
        }
        float clampedProgress = MathHelper.Clamp(AITimer / WaitTime, 0f, 1f);
        if (AITimer % 4 == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = Vector2.UnitY.RotatedBy(i * MathHelper.PiOver4 + Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f));
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.Center,
                    dir,
                    ProjectileType<HolyLightningSmall>(),
                    ai0: 0,
                    ai1: Main.rand.NextFloat(LengthAvg * 0.75f, LengthAvg * 1.25f) * clampedProgress
                    );
            }
        }

        if (AITimer < WaitTime)
        {
            Projectile.Opacity = clampedProgress;
            Projectile.scale = clampedProgress;
            Projectile.velocity *= 0.97f;
            AITimer++;
            return;
        }

        if (closestPlayer == null || !closestPlayer.IsAlive())
        {
            closestPlayer = LemonUtils.GetClosestPlayer(Projectile.Center);
        }

        if (closestPlayer != null && closestPlayer.IsAlive())
        {
            Projectile.MoveToPos(closestPlayer.Center, speed / 20f, speed / 20f, speed / 10f, speed / 10f);
        }

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustPerfect(
                Projectile.RandomPos(),
                DustType<StreakDust>(),
                Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(8, 12),
                newColor: Color.Gold
                ).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        float scale = Projectile.scale * 6 * (MathF.Sin(AITimer / 4f) + 3) * 0.25f;
        LemonUtils.DrawGlow(Projectile.Center, Color.LightYellow, Projectile.Opacity * 1f, scale);
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity * 1f, scale * 0.5f);
        return false;
    }
}
