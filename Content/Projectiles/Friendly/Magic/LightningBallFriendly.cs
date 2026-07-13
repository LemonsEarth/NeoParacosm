using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using Steamworks;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class LightningBallFriendly : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;
    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];
    ref float Duration => ref Projectile.ai[1];
    ref float LengthAvg => ref Projectile.ai[2];

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
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 1f;
        Projectile.DamageType = DamageClass.Magic;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        //target.AddBuff(BuffType<DeathflameDebuff>(), 30);
    }

    float speed = 1;
    Vector2 savedSpeed;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedSpeed = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
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
                    ProjectileType<HolyLightningFriendly>(),
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

        if (AITimer % 20 == 0 && Main.myPlayer == Projectile.owner)
        {
            int count = 0;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (count >= 3) break;
                if (npc.CanBeChasedBy() && Projectile.Center.DistanceSQ(npc.Center) < 500 * 500)
                {
                    Vector2 dir = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    float distance = (npc.Center - Projectile.Center).Length();
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center,
                        dir,
                        ProjectileType<HolyLightningFriendly>(),
                        ai0: 0,
                        ai1: distance
                        );

                    count++;
                }
            }
        }

        Projectile.velocity = savedSpeed;

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
