
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;

public class DeathbirdFeather : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("DeathbirdWingShader");

    int AITimer = 0;
    ref float TimeToFire => ref Projectile.ai[0];
    ref float IndicatorLength => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 600;
        Projectile.scale = 1f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            if (IndicatorLength == 0) IndicatorLength = 1;
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 1f);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { PitchRange = (0f, 0.2f), MaxInstances = 3 }, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 0);
        if (AITimer >= TimeToFire) Projectile.Kill();
        Projectile.velocity *= 0.93f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<DeathLaser>(), ai0: 0.25f, ai1: Projectile.velocity.ToRotation() - MathHelper.PiOver2);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public void DrawProjectile()
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        ShaderData.Shader.Parameters["uTime"].SetValue(AITimer);
        ShaderData.Shader.Parameters["tolerance"].SetValue(0.5f);
        ShaderData.Shader.Parameters["darkColorBoost"].SetValue(0f);
        ShaderData.Shader.Parameters["color"].SetValue(Color.White.ToVector4());
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;

        // First the "outline"/afterimage/effect wings
        ShaderData.Shader.Parameters["moveSpeed"].SetValue(0.75f);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);
        ShaderData.Shader.Parameters["moveSpeed"].SetValue(-0.75f);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

        ShaderData.Shader.Parameters["moveSpeed"].SetValue(0.75f);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        ShaderData.Shader.Parameters["moveSpeed"].SetValue(-0.75f);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

        }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        // Indicator
        Vector2 indicatorOrigin = new Vector2(texture.Width * 0.5f, texture.Height);
        float indicatorPercentComplete = AITimer / TimeToFire;
        float indicatorScale = indicatorPercentComplete * 5 * IndicatorLength;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * indicatorPercentComplete * 0.35f, Projectile.rotation, indicatorOrigin, new Vector2(1, indicatorScale), SpriteEffects.None, 0);

        this.QueueToShaderRenderer();
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
    }
}
