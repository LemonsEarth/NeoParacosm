
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Misc;

public class GravitySuckyProj : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("ShieldPulseShader");
    public override string Texture => "NeoParacosm/Common/Assets/Textures/Misc/Empty100Tex";

    int AITimer = 0;
    ref float distance => ref Projectile.ai[0];
    ref float strengthDenominator => ref Projectile.ai[1];
    ref float duration => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 120;
        Projectile.height = 120;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        if (AITimer <= duration)
        {
            foreach (var pl in Main.ActivePlayers)
            {
                if (pl.Distance(Projectile.Center) < distance)
                {
                    pl.velocity += pl.DirectionTo(Projectile.Center) * pl.Distance(Projectile.Center) / strengthDenominator;
                }
            }
        }
        Projectile.velocity = Vector2.Zero;
        AITimer++;
    }

    float speed = -2f;
    float cycleDuration = 100f;
    Color color = new Color(0.7f, 0.0f, 1f, 1f);

    public void DrawProjectile()
    {
        if (AITimer > cycleDuration / Math.Abs(speed))
        {
            return;
        }
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        ShaderData.Shader.Parameters["time"].SetValue(AITimer / cycleDuration);
        ShaderData.Shader.Parameters["alwaysVisible"].SetValue(false);
        ShaderData.Shader.Parameters["speed"].SetValue(speed);
        ShaderData.Shader.Parameters["colorMultiplier"].SetValue(4f);
        ShaderData.Shader.Parameters["color"].SetValue(color.ToVector4());
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 10, SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        this.QueueToShaderRenderer();
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
