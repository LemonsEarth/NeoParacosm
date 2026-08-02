
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class GravityRepelProjFriendly : ModProjectile
{
    public override string Texture => "NeoParacosm/Common/Assets/Textures/Misc/Empty100Tex";

    int AITimer = 0;
    ref float distance => ref Projectile.ai[0];
    ref float strength => ref Projectile.ai[1];
    ref float duration => ref Projectile.ai[2];

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
            for (int i = 0; i < distance / 20f; i++)
            {
                Vector2 dustPos = Projectile.Center;
                Vector2 dir = Vector2.UnitY.RotatedByRandom(6.28f);
                Dust.NewDustPerfect(dustPos, DustType<StreakDust>(), dir * Main.rand.NextFloat(distance / 40f, distance / 30f)).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (-0.9f, -0.8f), Volume = 0.7f, MaxInstances = 3 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (0.8f, 0.9f), Volume = 0.7f, MaxInstances = 3 }, Projectile.Center);
        }

        if (AITimer <= duration)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && npc.knockBackResist > 0f && npc.Distance(Projectile.Center) < distance)
                {
                    Vector2 dirFromProjectile = -(Projectile.Center - npc.Center).SafeNormalize(Vector2.Zero);
                    float distanceRatioClamped = MathHelper.Clamp(distance / npc.Distance(Projectile.Center), 0, 1);
                    float force = distanceRatioClamped * strength * npc.knockBackResist;
                    npc.velocity += dirFromProjectile * force;
                }
            }
        }
        if (AITimer / cycleDuration >= 1)
        {
            Projectile.Kill();
        }
        Projectile.velocity = Vector2.Zero;
        AITimer++;
    }

    float speed = 2f;
    float cycleDuration = 100f;
    Color color = new Color(0.7f, 0.0f, 1f, 1f);
    public override bool PreDraw(ref Color lightColor)
    {
        if (AITimer > cycleDuration / Math.Abs(speed))
        {
            return false;
        }
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        var shader = GameShaders.Misc["NeoParacosm:ShieldPulseShader"];
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Shader.Parameters["time"].SetValue(AITimer / cycleDuration);
        shader.Shader.Parameters["alwaysVisible"].SetValue(false);
        shader.Shader.Parameters["speed"].SetValue(speed);
        shader.Shader.Parameters["colorMultiplier"].SetValue(5f);
        shader.Shader.Parameters["color"].SetValue(color.ToVector4());
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        shader.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, distance / 50f, SpriteEffects.None, 0);
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
