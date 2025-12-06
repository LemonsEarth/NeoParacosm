
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Effect;

public class DragonRemainsPulseShield : ModProjectile
{
    public override string Texture => "NeoParacosm/Common/Assets/Textures/Misc/Empty100Tex";

    ref float AITimer => ref Projectile.ai[0];

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
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        var shader = GameShaders.Misc["NeoParacosm:ShieldPulseShader"];
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Shader.Parameters["time"].SetValue(0.99f); // constant size of shield
        shader.Shader.Parameters["noiseTimeX"].SetValue((AITimer * 5) / 100f);
        shader.Shader.Parameters["alwaysVisible"].SetValue(true);
        shader.Shader.Parameters["speed"].SetValue(1f);
        shader.Shader.Parameters["colorMultiplier"].SetValue(2f);
        float sinValue = ((float)Math.Sin(AITimer / 24) + 2) * 0.25f; // fades in and out on repeat
        shader.Shader.Parameters["color"].SetValue(Color.Yellow.ToVector4() * sinValue);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        shader.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 10, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        return false;
    }
}
