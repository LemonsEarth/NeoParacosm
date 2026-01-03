using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Researcher;

public class ElectroGasHostile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    int despawnTimer = 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 12;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 540;
        Projectile.penetrate = 6;
        Projectile.Opacity = 0f;
        Projectile.hide = true;
        Projectile.scale = 8;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        modifiers.ArmorPenetration += 100;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity *= 0.8f;
        return false;
    }

    public override void AI()
    {
        /*if (AITimer % 10 == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.RandomPos(), Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(1, 2), GoreType<RedSmokeGore>(), Main.rand.NextFloat(0.8f, 1.2f));
            }
        }*/
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

        Dust.NewDustPerfect(Projectile.RandomPos(16, 16), DustID.Electric).noGravity = true;

        if (AITimer % 60 == 0)
        {
            SoundEngine.PlaySound(SoundID.Item34 with { Pitch = 1f, Volume = 0.3f, MaxInstances = 2 }, Projectile.Center);
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
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["distance"].SetValue(1);
        shader.Shader.Parameters["color"].SetValue(new Vector4(0.5f, 0.8f, 1f, Projectile.Opacity));
        shader.Shader.Parameters["velocity"].SetValue(new Vector2(-Projectile.velocity.X * 0.001f, 0.5f));
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
