using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using ReLogic.Content;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Ice.EmperorColdsteel;

public class IceBurst : ModProjectile
{
    string TrailPath => Texture + "Trail";
    static Asset<Texture2D> TrailTexture;

    int AITimer = 0;
    int state = 0;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float WaitTime => ref Projectile.ai[1];
    ref float Speed => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
        TrailTexture = Request<Texture2D>(TrailPath);
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 9999;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            SoundEngine.PlaySound(SoundID.Item50 with { PitchRange = (0.4f, 0.6f) }, Projectile.Center);
            Projectile.frame = Main.rand.Next(0, 2);
        }
        if (AITimer % 3 == 0)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.IceRod, Scale: Main.rand.NextFloat(1.2f, 1.75f)).noGravity = true;
        }

        if (AITimer == WaitTime)
        {
            state = 1;
            SoundEngine.PlaySound(SoundID.Item25 with { PitchRange = (-0.4f, -0.2f) }, Projectile.Center);
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.Ice, 1.5f);
        }

        if (AITimer > WaitTime * 2)
        {
            Projectile.Kill();
            return;
        }
        if (AITimer < WaitTime * 0.5f)
        {
            Projectile.Opacity = AITimer / (WaitTime * 0.5f);
        }
        Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 1);
        //Projectile.rotation = MathHelper.ToRadians(Projectile.velocity.Length() * AITimer);
        Projectile.frame = state;
        AITimer++;
    }

    public override bool CanHitPlayer(Player target)
    {
        return state == 1;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.Ice, 1.5f);
        SoundEngine.PlaySound(SoundID.Item29 with { PitchRange = (-0.4f, -0.2f) }, Projectile.Center);
        if (state == 1 && LemonUtils.NotClient())
        {
            for (int i = 0; i < 8; i++)
            {
                float speed = i % 2 != 0 ? Speed * 1.4f : Speed;
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.Center,
                    Vector2.UnitY.RotatedBy(i * MathHelper.PiOver4) * speed,
                    ProjectileType<IceSpike>(),
                    ai0: TimeLeft
                    );
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
        {
            Vector2 origin = TrailTexture.Frame(1, 2, 0, Projectile.frame).Size() * 0.5f;
            Vector2 pos = Projectile.oldPos[i] + origin - Main.screenPosition;
            Main.EntitySpriteDraw(TrailTexture.Value, pos, TrailTexture.Frame(1, 2, 0, Projectile.frame), Color.LightBlue * 0.2f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        }
        LemonUtils.DrawGlow(Projectile.Center, Color.Cyan, 0.75f * Projectile.Opacity, Projectile.scale);
        return true;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
