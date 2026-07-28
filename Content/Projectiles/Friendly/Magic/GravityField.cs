using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class GravityField : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;
    int AITimer = 0;
    ref float Duration => ref Projectile.ai[0];
    ref float Power => ref Projectile.ai[1];
    ref float Scale => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 160;
        Projectile.height = 300;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
        Projectile.penetrate = 6;
        Projectile.Opacity = 0f;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.hide = true;
        Projectile.tileCollide = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override bool? CanHitNPC(NPC target)
    {
        return false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

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

        if (AITimer == 0)
        {
            if (Power == 0) Power = 1f;
            if (Scale == 0) Scale = 1f;
            Projectile.scale = Scale;
            Projectile.Resize(160 * Scale, 300 * Scale);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (-0.9f, -0.8f), Volume = 0.7f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (0.8f, 0.9f), Volume = 0.7f }, Projectile.Center);
        }

        if (AITimer < 5)
        {
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, (AITimer + 1) / 5f);
        }
        else if (Duration - AITimer < 15)
        {
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, (Duration - AITimer) / 15f);
        }

        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.CanBeChasedBy() && npc.knockBackResist != 0f)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox))
                {
                    npc.velocity.Y += npc.knockBackResist * 0.25f * Power;
                    npc.velocity.X *= (1 - npc.knockBackResist * 0.1f * Power);
                }
            }
        }

        Projectile.velocity = Vector2.Zero;
        if (AITimer >= Duration)
        {
            Projectile.Kill();
            return;
        }

        AITimer++;
    }

    Color color = new Color(0.7f, 0.0f, 1f, 0);
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() / 2;
        Vector2 drawPos = Projectile.Center;
        color.A = (byte)(Projectile.Opacity * 255);
        //Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, texture.Frame(1, 3, 0, 0), Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        var shader = GameShaders.Misc["NeoParacosm:GravityForceShader"];
        shader.Shader.Parameters["color"].SetValue(color.ToVector4());
        //shader.Shader.Parameters["distance"].SetValue(1);
        //shader.Shader.Parameters["color"].SetValue(new Vector4(1, 0, 0, Projectile.Opacity));
        //shader.Shader.Parameters["velocity"].SetValue(new Vector2(0, 0.5f));
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Main.EntitySpriteDraw(
            texture,
            drawPos - Main.screenPosition,
            null,
            Color.White * Projectile.Opacity,
            Projectile.rotation,
            drawOrigin,
            new Vector2((Projectile.width / 100f), (Projectile.height / 100f)) * Projectile.scale * 0.5f,
            SpriteEffects.None,
            0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }

    public override void OnKill(int timeLeft)
    {

    }
}
