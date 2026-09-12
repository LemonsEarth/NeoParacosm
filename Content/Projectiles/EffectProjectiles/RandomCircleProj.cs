
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.EffectProjectiles;

public class RandomCircleProj : ModProjectile, IShaderProjectile
{
    int AITimer = 0;

    public Color PulseColor { get; set; } = Color.White;
    public Entity EntityToFollow { get; set; } = null;

    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("OutlineShader");

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
        this.QueueToShaderRenderer();
        return false;
    }

    public void DrawProjectile()
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, drawPos - Vector2.UnitX * 30, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, drawPos + Vector2.UnitX * 30, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
    }

    public override void PostDraw(Color lightColor)
    {
    }
}
