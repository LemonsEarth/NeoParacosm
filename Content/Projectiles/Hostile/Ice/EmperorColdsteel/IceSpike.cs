using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using ReLogic.Content;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Ice.EmperorColdsteel;

public class IceSpike : ModProjectile
{
    string TrailPath => Texture + "Trail";
    static Asset<Texture2D> TrailTexture;

    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
        TrailTexture = Request<Texture2D>(TrailPath);
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
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
            SoundEngine.PlaySound(SoundID.Item50 with { PitchRange = (0.4f, 0.6f) }, Projectile.Center);
            Projectile.frame = Main.rand.Next(0, 2);
        }
        if (AITimer % 3 == 0)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.IceRod, Scale: Main.rand.NextFloat(1.2f, 1.75f)).noGravity = true;
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
            return;
        }
        Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 1);
        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Ice, Projectile.velocity.X, Projectile.velocity.Y, Scale: Main.rand.NextFloat(1.2f, 1.75f)).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
        {
            if (i % 4 == 0)
            {
                continue;
            }
            Color color = Color.Lerp(Color.Cyan, Color.Blue, (float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
            Vector2 origin = TrailTexture.Frame(1, 2, 0, Projectile.frame).Size() * 0.5f;
            Vector2 pos = Projectile.oldPos[i] + origin - Main.screenPosition;
            Main.EntitySpriteDraw(TrailTexture.Value, pos, TrailTexture.Frame(1, 2, 0, Projectile.frame), color * 0.2f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        }
        return true;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
