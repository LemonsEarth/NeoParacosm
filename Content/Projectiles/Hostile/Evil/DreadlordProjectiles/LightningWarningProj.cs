using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class LightningWarningProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float Duration => ref Projectile.ai[1];
    ref float Length => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.timeLeft = 999;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
    }

    List<Vector2> positions = new List<Vector2>();

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    Vector2 startPos = Vector2.Zero;

    public override void AI()
    {
        if (AITimer == 0)
        {
            startPos = Projectile.Center;
        }
        if (AITimer % 4 == 0)
        {
            Dust.NewDustDirect(Projectile.RandomPos(-16), 2, 2, DustType<StreakDust>()).velocity = Vector2.UnitY * 60;
        }
        Projectile.scale = AITimer / Duration * MathHelper.Clamp(Length / 2000f, 1, 5);
        Projectile.height = (int)(64 * Length / 128);

        Projectile.velocity = Vector2.Zero;
        if (AITimer > Duration)
        {
            Projectile.Kill();
        }
        AITimer++;
    }


    public override void OnKill(int timeLeft)
    {
        Projectile.NewProjectileDirect(
            Projectile.GetSource_Death(),
            startPos,
            Vector2.Zero,
            ProjectileType<IchorLightning>(),
            Projectile.damage,
            1f,
            -1,
            ai1: Length
            );
        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 5f, color: Color.Orange);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = 0; i < 1; i++)
        {

            Main.EntitySpriteDraw(ParacosmTextures.GlowBallTexture.Value,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White * Projectile.Opacity * 0.5f,
                0f,
                new Vector2(ParacosmTextures.GlowBallTexture.Width() * 0.5f, 0),
                new Vector2(1f, Length / 128 * Projectile.scale),
                SpriteEffects.None);
        }

        return false;
    }
}