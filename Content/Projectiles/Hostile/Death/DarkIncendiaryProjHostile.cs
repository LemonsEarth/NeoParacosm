using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Death;

public class DarkIncendiaryProjHostile : ModProjectile
{
    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];
    ref float PlayerID => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 52;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 999;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 1f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver4 * -MathF.Sign(oldVelocity.X));
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        //target.AddBuff(BuffType<DeathflameDebuff>(), 60);
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        Lighting.AddLight(Projectile.Center, 1, 1, 1);

        for (float i = 0; i < 2; i++)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(-8, -8), DustID.Ash, Scale: Main.rand.NextFloat(1, 1.2f), newColor: Color.Black).noGravity = true;
            //Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.GemDiamond, Vector2.Zero, newColor: Color.White, Scale: 1.2f).noGravity = true;
        }

        Projectile.rotation = MathHelper.ToRadians(Projectile.velocity.Length() * AITimer);

        Projectile.StandardAnimation(6, 4);

        if (AITimer > WaitTime)
        {
            Projectile.Kill();
        }

        Projectile.velocity.Y += 0.1f;
        Projectile.velocity.X *= 0.99f;

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        if (LemonUtils.NotClient())
        {
            for (int i = 0; i < 3; i++)
            {
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.RandomPos(),
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(2, 6),
                    ProjectileType<LingeringDeathflame>(),
                    ai0: PlayerID,
                    ai1: 300
                    );
            }
        }
        SoundEngine.PlaySound(SoundID.Item62 with { PitchRange = (-0.5f, -0.3f) }, Projectile.Center);
        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemDiamond, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), Scale: Main.rand.NextFloat(2.5f, 3.5f)).noGravity = true;
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Granite, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), newColor: Color.Black, Scale: Main.rand.NextFloat(2.5f, 3.5f)).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
        return true;
    }
}
