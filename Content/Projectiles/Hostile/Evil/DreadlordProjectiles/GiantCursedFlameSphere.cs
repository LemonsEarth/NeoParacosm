using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class GiantCursedFlameSphere : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("FireShader");

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
        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);
        if (SpeedUP == 0)
        {
            SpeedUP = 1f;
        }

        Projectile.StandardAnimation(6, 6);
        AITimer++;
    }

    public void DrawProjectile()
    {
        Texture2D texture = ParacosmTextures.Empty100Tex.Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        ShaderData.UseImage1(ParacosmTextures.NoiseTexture);
        ShaderData.UseColor(Color.Green * Projectile.Opacity);
        ShaderData.Shader.Parameters["flameHeightDownward"].SetValue(1); // Higher number lowers the height of the flame
        ShaderData.Shader.Parameters["moveVector"].SetValue(Vector2.UnitY); // Higher number lowers the height of the flame
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.75f, SpriteEffects.None, 0);
        ShaderData.UseColor(Color.White * Projectile.Opacity);
        ShaderData.Shader.Parameters["flameHeightDownward"].SetValue(1f);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.7f * 0.5f, SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        this.QueueToShaderRenderer();

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.CursedInferno, 600);
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
            LemonUtils.QuickPulse(Projectile, Projectile.Center, 3, 30, 5, Color.LightGreen);
            for (int i = 0; i < 16; i++)
            {
                LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.UnitY.RotatedBy(i * Angle) * 2, ProjectileType<CursedFlameSphere>(), ai1: SpeedUP);
            }
        }
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.CursedTorch, 2f);
    }
}
