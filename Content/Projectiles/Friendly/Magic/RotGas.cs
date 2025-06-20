using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using NeoParacosm.Content.Gores;
using NeoParacosm.Core.Systems;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class RotGas : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float RandomRot => ref Projectile.ai[1];
    int despawnTimer = 0;

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
        Projectile.timeLeft = 300;
        Projectile.penetrate = 6;
        Projectile.Opacity = 0f;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.hide = true;
        Projectile.scale = 8;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(ModContent.BuffType<CrimsonRotDebuff>(), 300);
    }

    public override void AI()
    {
        /*if (AITimer % 10 == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.RandomPos(), Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(1, 2), ModContent.GoreType<RedSmokeGore>(), Main.rand.NextFloat(0.8f, 1.2f));
            }
        }*/
        if (AITimer == 0)
        {
            RandomRot = Main.rand.NextFloat(0.5f, 3);
        }
        Projectile.velocity *= 0.98f;
        if (Projectile.timeLeft < 30)
        {
            despawnTimer++;
            Projectile.Opacity = MathHelper.Lerp(1, 0, despawnTimer / 30f);
        }

        if (AITimer < 30)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);

        }
        //Projectile.rotation += MathHelper.ToRadians(RandomRot);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() / 2;
        Vector2 drawPos = Projectile.Center;

        //Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, texture.Frame(1, 3, 0, 0), Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        var shader = GameShaders.Misc["NeoParacosm:GasShader"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer + Main.rand.NextFloat(1, 10));
        shader.Shader.Parameters["color"].SetValue(new Vector4(1, 0, 0, Projectile.Opacity));
        shader.Shader.Parameters["velocityX"].SetValue(-Projectile.velocity.X);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, new Vector2(Projectile.scale * 2, Projectile.scale), SpriteEffects.None, 0);
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
        
    }
}
