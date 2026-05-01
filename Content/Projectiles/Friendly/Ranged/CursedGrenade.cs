using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class CursedGrenade : ModProjectile
{
    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
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

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    float rotValue = 0;
    public override void AI()
    {
        if (AITimer == 0)
        {
            if (WaitTime == 0)
            {
                WaitTime = 90;
            }
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 1);

        for (float i = 0; i < 2; i++)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(-8, -8), DustID.CursedTorch, Scale: Main.rand.NextFloat(1, 1.2f), newColor: Color.Black).noGravity = true;
            //Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.GemDiamond, Vector2.Zero, newColor: Color.White, Scale: 1.2f).noGravity = true;
        }

        rotValue = MathHelper.Lerp(rotValue, Projectile.velocity.X * AITimer, 1 / 20f);
        Projectile.rotation = MathHelper.ToRadians(rotValue);

        if (AITimer > WaitTime)
        {
            Projectile.Kill();
        }

        if (AITimer > 30)
        {
            Projectile.velocity.Y += 0.2f;
        }

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
                    ProjectileID.CursedFlameFriendly,
                    Projectile.damage
                    );
            }
        }
        SoundEngine.PlaySound(SoundID.Item62 with { PitchRange = (-0.5f, -0.3f) }, Projectile.Center);
        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemEmerald, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.CursedTorch, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.LightGreen, Projectile.Opacity * 0.6f, Projectile.scale);
        return true;
    }
}
