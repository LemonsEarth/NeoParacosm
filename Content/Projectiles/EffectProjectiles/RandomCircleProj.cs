
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using System.IO;
using System.Linq;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.EffectProjectiles;

public class RandomCircleProj : ModProjectile
{
    int AITimer = 0;

    public Color PulseColor { get; set; } = Color.White;
    public Entity EntityToFollow { get; set; } = null;

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
        if (AITimer == 0)
        {

        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        var shader = GameShaders.Misc["NeoParacosm:OutlineShader"];
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        shader.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, drawPos - Vector2.UnitX * 30, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, drawPos + Vector2.UnitX * 30, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
