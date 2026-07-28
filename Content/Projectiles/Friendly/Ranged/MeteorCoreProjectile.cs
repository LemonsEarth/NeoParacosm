using Mono.Cecil;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using Terraria.Audio;
using static System.Net.Mime.MediaTypeNames;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class MeteorCoreProjectile : ModProjectile
{
    int AITimer = 0;
    ref float Duration => ref Projectile.ai[0];
    ref float SlowDownRate => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 44;
        Projectile.height = 44;
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

    public override bool? CanHitNPC(NPC target)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //target.AddBuff(BuffID.OnFire3, 60);
    }

    Vector2 originalVelocity;
    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.frame = Main.rand.Next(0, 3);
            originalVelocity = Projectile.velocity;
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 0);

        for (float i = 0; i < 2; i++)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(), DustID.GemTopaz, Scale: Main.rand.NextFloat(1, 1.2f)).noGravity = true;
            //Dust.NewDustPerfect(Projectile.RandomPos(4, 4), DustID.GemDiamond, Vector2.Zero, newColor: Color.White, Scale: 1.2f).noGravity = true;
        }

        if (AITimer > Duration)
        {
            Projectile.Kill();
            return;
        }

        Projectile.rotation = Projectile.velocity.Length();

        if (AITimer < Duration / 4f)
        {
            Projectile.velocity = Vector2.Lerp(originalVelocity, Vector2.Zero, (AITimer) / (Duration / 4f));
        }
        else if (AITimer >= Duration / 2f)
        {
            Projectile.velocity = Vector2.Zero;

            if (AITimer % 120 == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center,
                        Vector2.Zero,
                        ProjectileType<GravitySuckyProjFriendly>(),
                        ai0: 400,
                        ai1: 40,
                        ai2: 3
                        );
                }
            }
        }
        else
        {
            Projectile.velocity = Vector2.Zero;

        }

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item62 with { PitchRange = (-0.5f, -0.3f) }, Projectile.Center);
        for (int i = 0; i < 20; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.MeteorHead, Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 8), Scale: Main.rand.NextFloat(2.0f, 3.0f)).noGravity = true;
            Dust.NewDustPerfect(Projectile.RandomPos(), DustID.GemTopaz, Main.rand.NextVector2Circular(4f, 4f), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        }

        if (Main.myPlayer == Projectile.owner)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 trueVelocity = Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(15, 20);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromAI(),
                    Projectile.Center,
                    trueVelocity,
                    ProjectileType<MeteorFragment>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return true;
    }
}
