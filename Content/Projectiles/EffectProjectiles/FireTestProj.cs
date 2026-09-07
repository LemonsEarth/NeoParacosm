
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using System.IO;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.EffectProjectiles;

public class FireTestProj : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("FireShader");
    int AITimer = 0;
    ref float Timeleft => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];
    public override string Texture => ParacosmTextures.Empty100TexPath;

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
        Projectile.width = 200;
        Projectile.height = 200;
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
        this.QueueToShaderRenderer();
        return false;
    }

    public void DrawProjectile()
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        ShaderData.UseImage1(ParacosmTextures.NoiseTexture);
        ShaderData.UseColor(Color.Green);
        ShaderData.Shader.Parameters["flameHeightDownward"].SetValue(1);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Scale, SpriteEffects.None, 0);
        ShaderData.UseColor(Color.White);
        ShaderData.Shader.Parameters["flameHeightDownward"].SetValue(1f);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Scale * 0.5f, SpriteEffects.None, 0);
    }

    public override void PostDraw(Color lightColor)
    {
    }
}
