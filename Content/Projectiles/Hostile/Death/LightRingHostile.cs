using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using NeoParacosm.Core.Systems.Particles;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Death;

public class LightRingHostile : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("RingShader");

    int AITimer;
    int PreAITimer;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];
    ref float WaitTime => ref Projectile.ai[2];

    public override string Texture => ParacosmTextures.Empty100TexPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 14;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.timeLeft = 1200;

        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.penetrate = 10;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.scale = 0.5f;
        Projectile.Opacity = 0;
    }

    int timeToReachTarget => (int)(TimeLeft * 0.33f);
    int stationaryDuration => (int)(TimeLeft * 0.33f);

    public override bool PreAI()
    {
        if (PreAITimer < WaitTime)
        {
            if (PreAITimer == 0)
            {
                LemonUtils.ParticleBurst(
                    12,
                    Projectile.Center,
                    ParticleID.Fire,
                    5f, 5f,
                    0.6f, 0.9f,
                    Color.LightYellow
                );
                startPos = Projectile.Center;
                targetPos = Projectile.Center + Projectile.velocity * timeToReachTarget;
                Projectile.velocity = Vector2.Zero;
                Projectile.scale = Scale * 0.5f;
                Projectile.Resize((int)(100 * 0.5f * Scale), (int)(100 * 0.5f * Scale));
            }

            if (PreAITimer < 15)
            {
                Projectile.Opacity += 1 / 15f;
            }
            PreAITimer++;
            return false;
        }
        return true;
    }

    Vector2 startPos;
    Vector2 targetPos;
    public override void AI()
    {
        Projectile.rotation = MathHelper.ToRadians(AITimer * 6f);
        if (AITimer == 0)
        {
            LemonUtils.ParticleBurst(
                    12,
                    Projectile.Center,
                    ParticleID.Fire,
                    5f, 5f,
                    0.6f, 0.9f,
                    Color.LightYellow
                );
        }
        Dust.NewDustPerfect(Projectile.RandomPos(), DustID.GemTopaz, Vector2.Zero).noGravity = true;

        if (AITimer < timeToReachTarget)
        {
            Projectile.Center = Vector2.Lerp(startPos, targetPos, ((float)AITimer + 1) / timeToReachTarget);
        }
        else if (AITimer >= timeToReachTarget && AITimer < timeToReachTarget + stationaryDuration)
        {

        }
        else
        {
            int adjustedTimer = AITimer - (timeToReachTarget + stationaryDuration);
            Projectile.Center = Vector2.Lerp(targetPos, startPos, MathF.Min((float)adjustedTimer / timeToReachTarget, 1));
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }

        if (TimeLeft - AITimer < 15)
        {
            Projectile.Opacity -= 1 / 15f;
        }
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.ParticleBurst(
                    12,
                    Projectile.Center,
                    ParticleID.Fire,
                    3f, 3f,
                    0.6f, 0.9f,
                    Color.LightYellow
                );
    }


    public void DrawProjectile()
    {
        Texture2D texture = Projectile.GetTexture();
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        Vector2 drawOffset = new Vector2(Projectile.width, Projectile.height) * 0.5f;
        ShaderData.UseImage1(ParacosmTextures.NoiseTexture);

        for (int k = Projectile.oldPos.Length - 1; k >= 0; k--)
        {
            float afterImgOpacity = Projectile.Opacity * (1 - ((float)k / Projectile.oldPos.Length));
            ShaderData.UseOpacity(afterImgOpacity);
            ShaderData.UseColor(Color.LightYellow with { A = (byte)(afterImgOpacity * 255) } * afterImgOpacity);
            ShaderData.Apply();
            Vector2 afterimageDrawPos = Projectile.oldPos[k] + drawOffset - Main.screenPosition;
            Main.EntitySpriteDraw(texture, afterimageDrawPos, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale * 1f, SpriteEffects.None, 0);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        ProjectileShaderRenderer.Instance.Queue(this);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
