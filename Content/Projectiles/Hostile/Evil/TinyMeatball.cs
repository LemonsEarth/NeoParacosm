using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Projectiles.Effect;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil;

public class TinyMeatball : ModProjectile
{
    int AITimer = 0;
    ref float PosX => ref Projectile.ai[0];
    ref float PosY => ref Projectile.ai[1];
    ref float TimeToReach => ref Projectile.ai[2];

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
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.Opacity = 1f;
    }

    Vector2 savedPos;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedPos = Projectile.Center;
            if (TimeToReach == 0)
            {
                TimeToReach = 60;
            }
        }

        if (AITimer > TimeToReach)
        {
            Projectile.Kill();
        }
        Projectile.velocity = Vector2.Zero;
        Projectile.Center = Vector2.Lerp(savedPos, new Vector2(PosX, PosY), AITimer / TimeToReach);
        Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);
        Dust.NewDustDirect(Projectile.RandomPos(0, 0), 2, 2, DustID.RedMoss, 0, 0, Scale: Main.rand.NextFloat(1f, 2f)).noGravity = true;
        Dust.NewDustDirect(Projectile.RandomPos(0, 0), 2, 2, DustID.Crimson, 0, 0, Scale: Main.rand.NextFloat(1f, 2f)).noGravity = true;
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.LightBlue, Color.Transparent, BasicEffect, topDistance: Projectile.height / 2, bottomDistance: Projectile.height / 2, positionOffset: new Vector2(Projectile.width / 2, Projectile.height / 2));
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        LemonUtils.DrawGlow(Projectile.Center, Color.Red, Projectile.Opacity, Projectile.scale * 2);
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            Vector2 drawPos = Projectile.oldPos[i] + drawOrigin;
            Main.EntitySpriteDraw(texture,
                drawPos - Main.screenPosition,
                null,
                Color.Red * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length) * Projectile.Opacity,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length),
                SpriteEffects.None);
        }
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.UnitY * 10, ProjectileType<CrimsonLostSoul>(), ai0: 90, ai1: 240);
    }
}
