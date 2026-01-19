using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class CursedFlameSphere : PrimProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float SpeedUP => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 300;
        Projectile.scale = 1f;
    }

    float savedSpeed = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Item92 with { PitchRange = (2f, 2.3f), Volume = 0.75f}, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);
        if (SpeedUP == 0)
        {
            SpeedUP = 1f;
        }
        Projectile.velocity *= SpeedUP;
        var dust = Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.CursedTorch, Scale: 2f);
        dust.noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.LightBlue, Color.Transparent, BasicEffect);
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        Color color = Color.White;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            Vector2 drawPos = Projectile.oldPos[i] + drawOrigin;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, color * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length), Projectile.rotation, drawOrigin, Projectile.scale * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length), SpriteEffects.None);
        }
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.CursedTorch, 2f);
        /*for (int i = 0; i < 4; i++)
        {
            LemonUtils.QuickProj(Projectile, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(i * MathHelper.PiOver2) * (savedSpeed / 4f), ProjectileType<SavDroneProjectile>());
        }*/
    }
}
