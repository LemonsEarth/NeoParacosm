
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using System.IO;
using System.Linq;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Effect;

public class FireTestProj : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;

    int AITimer = 0;
    ref float Timeleft => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];
    ref float ColorMult => ref Projectile.ai[2];

    public Color PulseColor { get; set; } = Color.White;
    public Entity EntityToFollow { get; set; } = null;
    int entityType = -1;
    int entityID = -1;

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 9999;
    }

    public override void AI()
    {
        if (AITimer > Timeleft)
        {
            Projectile.Kill();
        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        var shader = GameShaders.Misc["NeoParacosm:FireShader"];
        shader.UseImage1(ParacosmTextures.NoiseTexture);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        shader.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Scale, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //LemonUtils.DrawGlow(Projectile.Center, Color.Black, Projectile.Opacity, Projectile.scale);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
