using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class IchorLightning : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float Length => ref Projectile.ai[1];

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
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }
    Color color = Color.White;
    Vector2 targetPos => Projectile.Center + Vector2.UnitY * Length;
    float random = 0;
    public override void AI()
    {
        if (AITimer == 0)
        {
            random = Main.rand.Next(1, 100);
            PunchCameraModifier mod1 = new PunchCameraModifier(Projectile.Center + Vector2.UnitY * (Length / 2), (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 12f, 10, 1000f, FullName);
            Main.instance.CameraModifiers.Add(mod1);
            Projectile.rotation = Projectile.Center.DirectionTo(targetPos).ToRotation();
            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { PitchRange = (-0.2f, 0.2f), MaxInstances = 1, Volume = 0.35f }, Projectile.Center);
            SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (-0.8f, -0.2f), MaxInstances = 0, Volume = 1f }, Projectile.Center);
            SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (-0.8f, -0.2f), MaxInstances = 0, Volume = 1f }, Projectile.Center);

            for (int j = 0; j < 10; j++)
            {
                Vector2 randVector = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustType<StreakDust>(), randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
                Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustType<StreakDust>(), randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f), newColor: Color.Gold).noGravity = true;
            }
            for (int j = 0; j < 10; j++)
            {
                Vector2 randVector = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                Dust.NewDustDirect(targetPos, 2, 2, DustType<StreakDust>(), randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
                Dust.NewDustDirect(targetPos, 2, 2, DustType<StreakDust>(), randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f), newColor: Color.Gold).noGravity = true;
            }

            //positions[0] = originalPos;
            //positions[1] = originalPos;

            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { PitchRange = (-0.2f, 0.2f), MaxInstances = 1, Volume = 0.35f }, targetPos);
            SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (-0.8f, -0.2f), MaxInstances = 0, Volume = 1f }, targetPos);
            SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (-0.8f, -0.2f), MaxInstances = 0, Volume = 1f }, targetPos);
        }
        Projectile.velocity = Vector2.Zero;
        if (Projectile.timeLeft < 30)
        {
            color = Color.Lerp(Color.Orange, Color.White, Projectile.timeLeft / 15f);
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 10f);
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

        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 5f, color: Color.Orange);
    }

    float lightningLength => Length / 100f;

    void DrawLightning(float randomMul = 1, float segCountMul = 1)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.Gold, Projectile.Opacity + 0.3f, Projectile.scale * 2);
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity + 0.3f, Projectile.scale);

        var shader = GameShaders.Misc["NeoParacosm:BigLightningShader"];
        shader.Shader.Parameters["lightningLength"].SetValue(lightningLength);
        shader.Shader.Parameters["segmentCount"].SetValue(3);
        shader.Shader.Parameters["time"].SetValue(random * randomMul);
        shader.Shader.Parameters["tolerance"].SetValue(0.04f);
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

        LemonUtils.DrawGlow(targetPos, Color.Gold, Projectile.Opacity + 0.3f, Projectile.scale * 2);
        LemonUtils.DrawGlow(targetPos, Color.White, Projectile.Opacity + 0.3f, Projectile.scale);

    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawLightning(1f, 1);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();
    }
}