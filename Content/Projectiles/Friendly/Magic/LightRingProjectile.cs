using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class LightRingProjectile : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("RingShader");

    int AITimer;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];

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

        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = 10;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.scale = 0.5f;
        Projectile.Opacity = 0;
    }

    Vector2 startPos;
    Vector2 targetPos;
    public override void AI()
    {
        int timeToReachTarget = (int)(TimeLeft * 0.33f);
        int stationaryDuration = (int)(TimeLeft * 0.33f);
        Projectile.rotation = MathHelper.ToRadians(AITimer * 6f);
        if (AITimer == 0)
        {
            Projectile.scale = Scale * 0.5f;
            Projectile.Resize((int)(100 * 0.5f * Scale), (int)(100 * 0.5f * Scale));
            startPos = Projectile.Center;
            targetPos = Projectile.Center + Projectile.velocity * timeToReachTarget;
            Projectile.velocity = Vector2.Zero;
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

        if (AITimer < 15)
        {
            Projectile.Opacity += 1 / 15f;
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
