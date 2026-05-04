using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class HolySpearFriendly : ModProjectile
{
    int AITimer = 0;
    ref float FireSpeed => ref Projectile.ai[0];
    ref float FallSpeed => ref Projectile.ai[1];
    ref float TimeBeforeFall => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 600;
        Projectile.scale = 1f;
    }

    float savedSpeed = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (0f, 0.3f), Volume = 0.75f }, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 0f);
        if (AITimer > TimeBeforeFall)
        {
            Projectile.velocity.Y += FallSpeed;
        }
        var dust = Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemTopaz, Scale: 1f);
        dust.noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.Gold, Color.Transparent, BasicEffect, topDistance: (int)(Projectile.height * 0.25f), bottomDistance: (int)(Projectile.height * 0.25f), positionOffset: Vector2.Zero);
        LemonUtils.DrawGlow(Projectile.Center, Color.LightYellow, Projectile.Opacity * 0.5f, Projectile.scale);
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = new Vector2(texture.Width * 0.83f, texture.Height * 0.5f);
        Color color = Color.Gold;
        /*for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            Vector2 drawPos = Projectile.oldPos[i] + drawOrigin;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        }*/
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        if (LemonUtils.NotClient())
        {
            LemonUtils.QuickProj(Projectile, Projectile.Center, -Vector2.UnitY * FireSpeed, ProjectileType<HolyFireFriendly>(), Projectile.damage * 0.5f, 1f, ai0: 30);
        }
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemTopaz, 2f);
        /*for (int i = 0; i < 4; i++)
        {
            LemonUtils.QuickProj(Projectile, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(i * MathHelper.PiOver2) * (savedSpeed / 4f), ProjectileType<SavDroneProjectile>());
        }*/
    }
}
