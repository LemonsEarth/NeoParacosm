using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class GiantCursedFlameSphere : PrimProjectile
{
    int AITimer = 0;
    ref float Angle => ref Projectile.ai[0];
    ref float SpeedUP => ref Projectile.ai[1];
    ref float TimeLeft => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.Opacity = 0f;
    }

    float savedSpeed = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            if (Angle == 0)
            {
                Angle = MathHelper.Pi / 8;
            }
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        }

        Projectile.velocity *= 0.97f;

        if (TimeLeft == 0)
        {
            TimeLeft = 60;
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }

        int pulseInterval = (int)TimeLeft / 4;
        if (AITimer % pulseInterval == 0)
        {
            Projectile.scale = 1.8f;
        }
        Projectile.scale = MathHelper.Lerp(Projectile.scale, 2f, 1 / 10f);

        Projectile.Opacity = AITimer / 15f;
        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);
        if (SpeedUP == 0)
        {
            SpeedUP = 1f;
        }
        Dust.NewDustDirect(Projectile.RandomPos(32, 32), 2, 2, DustID.GemEmerald, 0, Main.rand.NextFloat(-10, -5), Scale: Main.rand.NextFloat(2f, 4f)).noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
        Projectile.StandardAnimation(6, 6);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.LightBlue, Color.Transparent, BasicEffect, topDistance: Projectile.height / 2, bottomDistance: Projectile.height / 2, positionOffset: new Vector2(Projectile.width / 2, Projectile.height / 2));
        Texture2D texture = ParacosmTextures.NoiseTexture.Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        Color color = Color.White;

        var shader = GameShaders.Misc["NeoParacosm:SphereShader"];
        shader.UseImage1(ParacosmTextures.NoiseTexture);
        shader.Shader.Parameters["distance"].SetValue(0.7f);
        shader.Shader.Parameters["borderWidth"].SetValue(0.3f);
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile(effect: shader.Shader);
        Main.spriteBatch.Draw(texture,
            new Rectangle((int)(Projectile.Center.X - Main.screenPosition.X), (int)(Projectile.Center.Y - Main.screenPosition.Y), (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale)),
            null,
            color * Projectile.Opacity,
            Projectile.rotation,
            drawOrigin,
            SpriteEffects.None,
            0);
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        SoundEngine.PlaySound(SoundID.Item14 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<PulseEffect>(), ai0: 1f, ai1: 10, ai2: 5);
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.CursedTorch, 2f);
        for (int i = 0; i < 16; i++)
        {
            LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.UnitY.RotatedBy(i * Angle) * 2, ProjectileType<CursedFlameSphere>(), ai1: SpeedUP);
        }
    }
}
