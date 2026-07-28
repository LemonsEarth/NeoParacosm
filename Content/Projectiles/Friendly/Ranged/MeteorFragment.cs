using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class MeteorFragment : ModProjectile
{
    int AITimer = 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 1f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 60);
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.frame = Main.rand.Next(0, 3);
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 0);

        for (float i = 0; i < 2; i++)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(), DustID.MeteorHead, Scale: Main.rand.NextFloat(1, 1.2f)).noGravity = true;
            //Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.GemDiamond, Vector2.Zero, newColor: Color.White, Scale: 1.2f).noGravity = true;
        }

        Projectile.rotation = MathHelper.ToRadians(MathHelper.ToRadians(AITimer * 6));


        Projectile.velocity.Y += 0.2f;


        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item62 with { PitchRange = (-0.5f, -0.3f) }, Projectile.Center);
        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.MeteorHead, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), Scale: Main.rand.NextFloat(2.0f, 3.0f)).noGravity = true;
            Dust.NewDustPerfect(Projectile.RandomPos(), DustID.GemTopaz, Main.rand.NextVector2Circular(2f, 2f), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        }

        if (Main.myPlayer == Projectile.owner)
        {
            LemonUtils.QuickProj(
                Projectile,
                Projectile.Center,
                new Vector2(10, 0), // velocity x is used as timeleft
                ProjectileType<InvisibleProjectileFriendly>(),
                ai0: -1,
                ai1: 48,
                ai2: 48
                );
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return true;
    }
}
