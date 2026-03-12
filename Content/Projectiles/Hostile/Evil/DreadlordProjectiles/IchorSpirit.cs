using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class IchorSpirit : ModProjectile
{
    int AITimer = 0;
    ref float Duration => ref Projectile.ai[0];
    ref float ChaseSpeed => ref Projectile.ai[1];
    ref float AttackInterval => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.Opacity = 0f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    Player player = null;
    int despawnTimer = 0;
    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.IchorTorch, 3f);
        }

        if (AITimer > Duration)
        {
            Projectile.Kill();
        }

        player = LemonUtils.GetClosestPlayer(Projectile.Center, 2000);

        if (player == null || !player.Alive())
        {
            Projectile.Kill();
            return;
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 1);
        Projectile.StandardAnimation(6, 5);

        if (AITimer < 60)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.scale = MathHelper.Lerp(0, 1, AITimer / 60f);
            Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 60f);
            AITimer++;
            return;
        }

        if (AITimer < Duration - 60)
        {
            Projectile.velocity = Projectile.DirectionTo(player.Center) * ChaseSpeed;
            if (AITimer % AttackInterval == 0)
            {
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.oldPos[20] + new Vector2(Projectile.width, Projectile.height) * 0.5f,
                        Vector2.Zero,
                        ProjectileType<IchorFlamethrower>(),
                        ai0: 30
                        );
                }
            }
        }
        else
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.scale = MathHelper.Lerp(1, 0, despawnTimer / 60f);
            Projectile.Opacity = MathHelper.Lerp(1, 0, despawnTimer / 60f);
            despawnTimer++;
        }

        AITimer++;
    }

    public override bool CanHitPlayer(Player target)
    {
        return AITimer > 60 && AITimer < Duration - 60;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //LemonUtils.DrawGlow(Projectile.Bottom, Color.Gold, Projectile.Opacity, Projectile.scale);
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle sourceRect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        Vector2 drawPos = Projectile.Bottom - Main.screenPosition;
        Vector2 drawOrigin = new Vector2(sourceRect.Width * 0.5f, sourceRect.Height);
        float scaleX = (MathF.Sin(AITimer / 12f) * 0.05f + 1) * Projectile.scale;
        // 1.25 when 0
        float scaleY = (MathF.Sin(AITimer / 24f + MathHelper.PiOver2) * 0.2f + 1) * Projectile.scale;

        Main.EntitySpriteDraw(texture, drawPos, sourceRect, Color.White, Projectile.rotation, drawOrigin, new Vector2(scaleX, scaleY), SpriteEffects.None, 0);
        return false;
    }
}
