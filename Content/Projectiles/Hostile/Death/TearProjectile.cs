using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Death;

public class TearProjectile : PrimProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float SpeedUP => ref Projectile.ai[1];
    ref float Duration => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
    }

    float savedSpeed = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Drip with { PitchRange = (0f, 1f), Volume = 0.5f}, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item45 with { PitchRange = (0.2f, 0.5f), Volume = 0.5f}, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 0f);
        if (SpeedUP == 0)
        {
            SpeedUP = 1f;
        }

        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        Projectile.velocity *= SpeedUP;
        var dust = Dust.NewDustDirect(Projectile.RandomPos(0, -32), 2, 2, DustID.GemDiamond, Scale: 1.2f);
        dust.noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.StandardAnimation(6, 3);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Transparent, BasicEffect, topDistance: 8, bottomDistance: 8);
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Frame(1, 3, 0, 0).Size() * 0.5f;
        Color color = Color.White;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            Vector2 drawPos = Projectile.oldPos[i] + drawOrigin;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, texture.Frame(1, 3, 0, 0), color * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length), Projectile.rotation, drawOrigin, Projectile.scale * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length), SpriteEffects.None);
        }
        //LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 1f);
        /*for (int i = 0; i < 4; i++)
        {
            LemonUtils.QuickProj(Projectile, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(i * MathHelper.PiOver2) * (savedSpeed / 4f), ProjectileType<SavDroneProjectile>());
        }*/
    }
}
