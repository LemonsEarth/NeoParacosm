using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class HomingStar : PrimProjectile
{
    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];
    ref float TimeLeft => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.friendly = true;
        Projectile.timeLeft = 9999;
        Projectile.penetrate = 3;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.scale = 0.75f;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Generic;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    Vector2 startVelocity;
    NPC closestNPC;
    public override void AI()
    {
        Player player = Projectile.GetOwner();

        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.5f, PitchRange = (0.8f, 1f) }, Projectile.Center);

            startVelocity = Projectile.velocity;
            Projectile.localNPCHitCooldown = (int)WaitTime;
        }

        if (AITimer < WaitTime)
        {
            Projectile.velocity = Vector2.Lerp(startVelocity, Vector2.Zero, AITimer / WaitTime);
        }
        else if (AITimer == WaitTime)
        {
            closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, 600);
        }
        else
        {
            if (closestNPC != null && closestNPC.active)
            {
                Projectile.MoveToPos(closestNPC.Center, 0.1f, 0.1f, 0.8f, 0.8f);
            }
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
            return;
        }

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        Dust.NewDustPerfect(Projectile.Center, DustID.GemEmerald);
        LemonUtils.DustCircle(Projectile.Center, 8, 3, DustID.GemDiamond, 2f);
        SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.1f, PitchRange = (0.2f, 0.4f) }, Projectile.Center);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(
            Projectile,
            Color.White,
            Color.Transparent,
            BasicEffect,
            topVertexRotation: -MathHelper.PiOver2,
            bottomVertexRotation: MathHelper.PiOver2,
            topDistance: 8,
            bottomDistance: 8
            );
        Texture2D texture = Projectile.GetTexture();
        LemonUtils.DrawGlow(Projectile.Center, Color.White, 0.5f, Projectile.scale * 2);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        LemonUtils.DrawGlow(Projectile.Center, Color.White, 1f, Projectile.scale);
        return false;
    }
}
