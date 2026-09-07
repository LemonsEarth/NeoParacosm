using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Content.Projectiles;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class ExplodingIchorSphere : PrimProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("FireShader");
    int AITimer = 0;
    ref float FlameSpeed => ref Projectile.ai[0];
    ref float FlameSlowDownRate => ref Projectile.ai[1];
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
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
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
            if (FlameSpeed == 0)
            {
                FlameSpeed = 10;
            }
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 5 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 5 }, Projectile.Center);
        }

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
        Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 0.2f);
        //Dust.NewDustDirect(Projectile.RandomPos(32, 32), 2, 2, DustID.GemEmerald, 0, Main.rand.NextFloat(-10, -5), Scale: Main.rand.NextFloat(2f, 4f)).noGravity = true;
        //Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
        Projectile.StandardAnimation(6, 6);
        AITimer++;
    }

    public void DrawProjectile()
    {
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.LightBlue, Color.Transparent, BasicEffect, topDistance: Projectile.height / 2, bottomDistance: Projectile.height / 2, positionOffset: new Vector2(Projectile.width / 2, Projectile.height / 2));
        Texture2D texture = ParacosmTextures.Empty100Tex.Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        ShaderData.UseImage1(ParacosmTextures.NoiseTexture);
        ShaderData.UseColor(Color.Gold * Projectile.Opacity);
        ShaderData.Shader.Parameters["flameHeightDownward"].SetValue(1); // Higher number lowers the height of the flame
        ShaderData.Shader.Parameters["moveVector"].SetValue(Vector2.UnitY); // Higher number lowers the height of the flame
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.75f, SpriteEffects.None, 0);
        ShaderData.UseColor(Color.White * Projectile.Opacity);
        ShaderData.Shader.Parameters["flameHeightDownward"].SetValue(1f);
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.7f * 0.5f, SpriteEffects.None, 0);
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        ProjectileShaderRenderer.Instance.Queue(this);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.Ichor, 600);
    }

    public override void OnKill(int timeLeft)
    {
        if (!Main.dedServ)
        {
            Vector2 movedPos = Vector2.Lerp(Projectile.Center, Main.LocalPlayer.Center, 0.8f);
            SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 5 }, movedPos);
            SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 5 }, movedPos);
            SoundEngine.PlaySound(SoundID.Item14 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 5 }, movedPos);
        }
        if (LemonUtils.NotClient())
        {
            LemonUtils.QuickPulse(Projectile, Projectile.Center, 2, 20, 5, Color.Gold * 0.5f);
            Spawn_IchorFlames(Projectile.Center, Vector2.UnitY.RotatedByRandom(6.28f), angle: MathHelper.Pi, minSpeed: FlameSpeed - 2, maxSpeed: FlameSpeed + 2, iterations: 30);
        }
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.CursedTorch, 2f);
    }

    void Spawn_IchorFlames(Vector2 position, Vector2 direction, float angle = MathHelper.Pi / 16f, float minSpeed = 45, float maxSpeed = 55, float duration = 30, float slowDownRate = 0.97f, float turningAngle = MathHelper.Pi / 32, int iterations = 6)
    {
        for (int i = 0; i < iterations; i++)
        {
            LemonUtils.QuickProj(
                Projectile,
                position,
                direction.RotatedBy(Main.rand.NextFloat(-angle, angle)) * Main.rand.NextFloat(minSpeed, maxSpeed),
                ProjectileType<IchorFlamethrower>(),
                ai0: duration,
                ai1: FlameSlowDownRate,
                ai2: Main.rand.NextFloat(-turningAngle, turningAngle)
                );
        }
    }
}
