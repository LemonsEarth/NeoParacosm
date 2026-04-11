using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public abstract class TargetedLightning : ModProjectile
{
    protected int AITimer = 0;
    protected ref float Delay => ref Projectile.ai[0];
    protected Vector2 targetPos
    {
        get
        {
            return new Vector2(Projectile.ai[1], Projectile.ai[2]);
        }
        set
        {
            Projectile.ai[1] = value.X;
            Projectile.ai[2] = value.Y;
        }
    }

    /// <summary>
    /// Should be the distance between the Projectile Center and Target Position, divided by 100 (width and height of the empty texture used to draw the lightning)
    /// </summary>
    float lightningLength = 0;

    protected Vector2 originalPos = Vector2.Zero;
    protected virtual Color ShineColor => Color.White;
    protected virtual Color DarkColor => Color.Yellow;
    Color currentColor = Color.White;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
    }

    float random = 0;
    public override void AI()
    {
        if (AITimer == 0)
        {
            lightningLength = Projectile.Center.Distance(targetPos) / 100f;
            random = Main.rand.Next(1, 100);
            Projectile.rotation = Projectile.Center.DirectionTo(targetPos).ToRotation();
        }
        if (AITimer < Delay)
        {
            AITimer++;
            Projectile.timeLeft = 30;
            return;
        }
        if (AITimer == Delay)
        {
            LemonUtils.QuickScreenShake(Main.player[Projectile.owner].Center, 10, 16, 15, 500);
            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { PitchRange = (-0.2f, 0.2f), MaxInstances = 1, Volume = 0.35f }, Projectile.Center);
            SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (-0.2f, 0.2f), MaxInstances = 0, Volume = 0.75f }, Projectile.Center);

            for (int j = 0; j < 20; j++)
            {
                Vector2 randVector = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
                Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
                Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustID.GemDiamond, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
                Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustID.GemAmber, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            }
        }
        Projectile.velocity = Vector2.Zero;
        if (Projectile.timeLeft < 30)
        {
            currentColor = Color.Lerp(DarkColor, ShineColor, Projectile.timeLeft / 15f);
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 5f);
        }
        //Main.NewText(currentColor);
        AITimer++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = float.NaN;
        Vector2 endPos = targetPos;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPos, Projectile.width, ref _);
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 5f, color: ShineColor);
    }

    void DrawLightning(float randomMul = 1, float segCountMul = 1)
    {
        LemonUtils.DrawGlow(Projectile.Center, DarkColor, Projectile.Opacity + 0.3f, Projectile.scale * 2);
        LemonUtils.DrawGlow(Projectile.Center, ShineColor, Projectile.Opacity + 0.3f, Projectile.scale);
        var shader = GameShaders.Misc["NeoParacosm:BigLightningShader"];
        shader.Shader.Parameters["lightningLength"].SetValue(lightningLength);
        shader.Shader.Parameters["segmentCount"].SetValue(3);
        shader.Shader.Parameters["time"].SetValue(random * randomMul);
        shader.Shader.Parameters["tolerance"].SetValue(0.04f);
        shader.Shader.Parameters["amplitudeMult"].SetValue(0.2f); // empty texture is much larger than weapon sprite, so we're making the lightning smaller
        shader.UseOpacity(Projectile.Opacity);
        shader.UseColor(currentColor * Projectile.Opacity);
        shader.Apply();

        Vector2 lightningScale = new(lightningLength, 1);
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile(effect: shader.Shader);
        Main.EntitySpriteDraw(
            ParacosmTextures.Empty100Tex.Value,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White,
            Projectile.rotation,
            Vector2.UnitY * ParacosmTextures.Empty100Tex.Height() * 0.5f,
            lightningScale,
            SpriteEffects.None);

        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();

        LemonUtils.DrawGlow(targetPos, DarkColor, Projectile.Opacity + 0.3f, Projectile.scale * 2);
        LemonUtils.DrawGlow(targetPos, ShineColor, Projectile.Opacity + 0.3f, Projectile.scale);

    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (AITimer < Delay)
        {
            return false;
        }
        DrawLightning(1f, 1);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();
    }
}