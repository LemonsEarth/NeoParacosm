using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class CrimsonCloud : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    int despawnTimer = 0;
    bool released = false;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.penetrate = -1;
        Projectile.Opacity = 0f;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.hide = true;
        Projectile.scale = 1;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Magic;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //target.AddBuff(BuffType<CrimsonRotDebuff>(), 300);
    }

    float savedScale = 1f;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (!player.channel) released = true;

        if (player.channel && !released)
        {
            Projectile.timeLeft = 180;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / (8 * (Projectile.scale + 1));
            }
            if (AITimer % 10 == 0)
            {
                Projectile.netUpdate = true;
            }
            float aiTimerClamped = MathHelper.Clamp(AITimer, 0, 300);
            Projectile.scale = MathHelper.Lerp(0.1f, 8f, aiTimerClamped / 300f);
            Projectile.Resize((int)(Projectile.scale * 24), (int)(Projectile.scale * 24));
            savedScale = Projectile.scale;
        }

        if (released)
        {
            Projectile.scale = MathHelper.Lerp(savedScale, 0f, (180 - Projectile.timeLeft) / 180f);
            Projectile.Resize((int)(Projectile.scale * 24), (int)(Projectile.scale * 24));
            Projectile.velocity *= 0.99f;
        }

        if (Projectile.timeLeft < 30)
        {
            despawnTimer++;
            Projectile.Opacity = MathHelper.Lerp(1, 0, despawnTimer / 30f);
        }

        if (AITimer < 30)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center;
        Vector2 drawOrigin = texture.Size() / 2;

        //Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, texture.Frame(1, 3, 0, 0), Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        Main.spriteBatch.End();
        var shader = GameShaders.Misc["NeoParacosm:GasShader"];
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        for (int i = 1; i <= 3; i++)
        {
            shader.Shader.Parameters["uTime"].SetValue(AITimer);
            shader.Shader.Parameters["distance"].SetValue(0.8f);
            shader.Shader.Parameters["tolerance"].SetValue(0.05f);
            shader.Shader.Parameters["velocity"].SetValue(new Vector2(0.4f * i, 0));
            shader.Shader.Parameters["color"].SetValue(new Vector4(0.33f * i, 0, 0, Projectile.Opacity));
            shader.Apply();
            float sinValue = ((float)Math.Sin(AITimer / 24) + 8) * 0.2f;
            float cosValue = ((float)Math.Cos(AITimer / 24) + 8) * 0.25f;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, new Vector2(Projectile.scale * 1.5f * sinValue, Projectile.scale * 1 * cosValue), SpriteEffects.None, 0);
        }
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < savedScale; i++)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI("Friendly"), Projectile.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * 10, ProjectileID.CultistBossLightningOrbArc, Projectile.damage, 1f, Projectile.owner, Main.rand.NextFloat(0, 6.28f));
        }
        //LemonUtils.QuickProj(Projectile, Projectile.RandomPos(), Vector2.UnitY * 10, ProjectileID.CultistBossLightningOrbArc, ai0: Main.rand.NextFloat(0, 6.28f));
    }
}
