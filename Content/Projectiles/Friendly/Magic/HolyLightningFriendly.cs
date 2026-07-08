using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class HolyLightningFriendly : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;
    int AITimer = 0;
    ref float DontDrawGlow => ref Projectile.ai[0];
    ref float Length => ref Projectile.ai[1];
    ref float TimeLeftMultiplier => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
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
        Projectile.localNPCHitCooldown = 60;
        Projectile.tileCollide = false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        //target.AddBuff(BuffID.Ichor, 300);
    }

    Color color = Color.White;
    Vector2 targetPos => Projectile.Center + savedVelocity * Length;
    float random = 0;
    Vector2 savedVelocity;
    public override void AI()
    {
        if (AITimer == 0)
        {
            if (TimeLeftMultiplier <= 0) TimeLeftMultiplier = 1;
            Projectile.timeLeft = (int)(Projectile.timeLeft * TimeLeftMultiplier);
            savedVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            random = Main.rand.Next(1, 100);
            PunchCameraModifier mod1 = new PunchCameraModifier(Projectile.Center + Vector2.UnitY * (Length / 2), (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 10f, 6f, 10, 1000f, FullName);
            Main.instance.CameraModifiers.Add(mod1);
            Projectile.rotation = Projectile.Center.DirectionTo(targetPos).ToRotation();

            SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (0.5f, 0.8f), MaxInstances = 10, Volume = 0.6f }, targetPos);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { PitchRange = (1f, 1.2f), Volume = 0.5f }, Projectile.Center);
        }
        Projectile.velocity = Vector2.Zero;
        if (Projectile.timeLeft < 30 * TimeLeftMultiplier)
        {
            color = Color.Lerp(Color.Gold, Color.White, Projectile.timeLeft / (15f * TimeLeftMultiplier));
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / (10f * TimeLeftMultiplier));
        }
        AITimer++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = float.NaN;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, targetPos, Projectile.width, ref _);
    }

    public override void OnKill(int timeLeft)
    {

        //LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 5f, color: Color.Orange);
    }

    float lightningLength => Length / 100f;

    void DrawLightning(float randomMul = 1, float segCountMul = 1)
    {
        if (DontDrawGlow == 0)
        {
            LemonUtils.DrawGlow(Projectile.Center, Color.LightYellow, Projectile.Opacity, Projectile.scale * Length / 100f);
            LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale * 0.5f * Length / 100f);
        }

        var shader = GameShaders.Misc["NeoParacosm:BigLightningShader"];
        shader.Shader.Parameters["lightningLength"].SetValue(lightningLength);
        shader.Shader.Parameters["segmentCount"].SetValue(6);
        shader.Shader.Parameters["time"].SetValue(random * randomMul);
        shader.Shader.Parameters["tolerance"].SetValue(0.02f);
        shader.Shader.Parameters["amplitudeMult"].SetValue(0.2f); // empty texture is much larger than weapon sprite, so we're making the lightning smaller
        shader.UseOpacity(Projectile.Opacity);
        shader.UseColor(color * Projectile.Opacity);
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
            LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();

        //LemonUtils.DrawGlow(targetPos, Color.Gold, Projectile.Opacity + 0.3f, Projectile.scale * 2);
        //LemonUtils.DrawGlow(targetPos, Color.White, Projectile.Opacity + 0.3f, Projectile.scale);

    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (AITimer <= 2)
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