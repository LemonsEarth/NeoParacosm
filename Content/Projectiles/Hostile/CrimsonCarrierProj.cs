using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Common.Utils.Prim;
using ReLogic.Content;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile;

public class CrimsonCarrierProj : ModProjectile
{
    string GlowMask_Path => Texture + "_Glow";
    static Asset<Texture2D> GlowMask;

    ref float AITimer => ref Projectile.ai[0];

    static BasicEffect BasicEffect;

    public override void Load()
    {
        if (Main.dedServ) return;
        Main.RunOnMainThread(() =>
        {
            BasicEffect = new BasicEffect(PrimHelper.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };
        });
    }

    public override void Unload()
    {
        if (Main.dedServ) return;
        Main.RunOnMainThread(() =>
        {
            BasicEffect?.Dispose();
            BasicEffect = null;
        });
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        GlowMask = Request<Texture2D>(GlowMask_Path);
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
        }

        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 7;
        }

        LemonUtils.DustCircle(Projectile.Center, 4, 3, DustID.Crimson);
        Projectile.rotation = MathHelper.ToRadians(AITimer * 6);
        Projectile.velocity *= 1.05f;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), ProjectileID.ViciousPowder, 0, 1);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Main.dedServ) return true;
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.DarkRed, Color.Transparent, BasicEffect);

        return true;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.EntitySpriteDraw(GlowMask.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Color.White, Projectile.rotation, GlowMask.Value.Size() * 0.5f, 1f, SpriteEffects.None);
    }
}
