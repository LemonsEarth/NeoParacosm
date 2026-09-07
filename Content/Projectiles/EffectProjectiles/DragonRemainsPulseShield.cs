
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.EffectProjectiles;

public class DragonRemainsPulseShield : ModProjectile, IShaderProjectile
{
    public override string Texture => "NeoParacosm/Common/Assets/Textures/Misc/Empty100Tex";

    ref float AITimer => ref Projectile.ai[0];

    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("ShieldPulseShader");

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
        if (NPC.downedBoss2)
        {
            Projectile.Kill();
        }
        Projectile.velocity = Vector2.Zero;
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        this.QueueToShaderRenderer();
        return false;
    }

    public void DrawProjectile()
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        ShaderData.Shader.Parameters["time"].SetValue(0.99f);
        ShaderData.Shader.Parameters["noiseTimeX"].SetValue((AITimer * 5) / 100f);
        ShaderData.Shader.Parameters["alwaysVisible"].SetValue(true);
        ShaderData.Shader.Parameters["speed"].SetValue(1f);
        ShaderData.Shader.Parameters["colorMultiplier"].SetValue(2f);
        float sinValue = ((float)Math.Sin(AITimer / 24) + 2) * 0.25f;
        ShaderData.Shader.Parameters["color"].SetValue(Color.Yellow.ToVector4() * sinValue);
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 10, SpriteEffects.None, 0);
    }

    public override void PostDraw(Color lightColor)
    {
    }
}
