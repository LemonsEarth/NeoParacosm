using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class GiantCursedFlameSphereFriendly : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("FireShader");

    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];
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
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.Opacity = 0f;
        Projectile.DamageType = DamageClass.Magic;
    }

    Vector2 savedVelocity;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedVelocity = Projectile.velocity;
            SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
            Projectile.velocity = Vector2.Zero;
        }

        if (TimeLeft == 0)
        {
            TimeLeft = 60;
        }

        if (SpeedUP == 0)
        {
            SpeedUP = 1f;
        }

        if (AITimer > TimeLeft + WaitTime)
        {
            Projectile.Kill();
        }

        int pulseInterval = (int)TimeLeft / 4;
        if (AITimer % pulseInterval == 0)
        {
            Projectile.scale = 1.8f;
        }
        Projectile.scale = MathHelper.Lerp(Projectile.scale, 2f, 1 / 10f);
        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);
        //Dust.NewDustDirect(Projectile.RandomPos(32, 32), 2, 2, DustID.GemEmerald, 0, Main.rand.NextFloat(-10, -5), Scale: Main.rand.NextFloat(2f, 4f)).noGravity = true;
        //Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
        Projectile.StandardAnimation(6, 6);

        Projectile.Opacity = AITimer / WaitTime;
        if (AITimer < WaitTime)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = Projectile.GetOwner().Center + Projectile.GetOwner().DirectionTo(Main.MouseWorld) * 181;
            }
            if (AITimer % 5 == 0)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = Vector2.Zero;
            AITimer++;
            return;
        }

        if (AITimer == WaitTime)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.GetOwner().DirectionTo(Main.MouseWorld) * savedVelocity.Length();
            }
            Projectile.netUpdate = true;
        }

        Projectile.velocity *= 0.95f;
        AITimer++;
    }

    public void DrawProjectile()
    {
        Texture2D texture = ParacosmTextures.Empty100Tex.Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        ShaderData.UseImage1(ParacosmTextures.NoiseTexture);
        ShaderData.Shader.Parameters["uTime"].SetValue(AITimer / 100f);

        ShaderData.UseColor(Color.Green * Projectile.Opacity);
        ShaderData.Shader.Parameters["flameHeightDownward"].SetValue(1f); // Higher number lowers the height of the flame
        ShaderData.Shader.Parameters["moveVector"].SetValue(Vector2.UnitY); // Higher number lowers the height of the flame
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.75f, SpriteEffects.None, 0);

        ShaderData.Shader.Parameters["uColor"].SetValue((Color.White * Projectile.Opacity).ToVector4());
        ShaderData.UseColor(Color.White * Projectile.Opacity);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.85f, SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        ProjectileShaderRenderer.Instance.Queue(this);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.CursedInferno, 600);
    }

    public override void OnKill(int timeLeft)
    {
        if (!Main.dedServ)
        {
            Vector2 movedPos = Projectile.Center;
            SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
            SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
            SoundEngine.PlaySound(SoundID.Item14 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
        }
        if (Main.myPlayer == Projectile.owner)
        {
            /*LemonUtils.QuickPulse(Projectile, Projectile.Center, 3, 30, 5, Color.LightGreen);
            for (int i = 0; i < 16; i++)
            {
                LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.UnitY.RotatedBy(i * MathHelper.Pi / 8f) * 2, ProjectileType<CursedFlameSphereFriendly>(), ai1: SpeedUP);
            }*/
        }
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.CursedTorch, 2f);
    }
}
