using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;
using System.Threading;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Particles;

public class GlowyParticle : ParticleType
{
    public override string TexturePath => ParacosmTextures.GlowBallTexturePath;

    public override void OnSpawn(ref Particle particle)
    {
        particle.opacity = 0f;
    }

    public override void Update(ref Particle particle)
    {
        float duration = particle.data[0];
        float fadeInDuration = particle.data[1];
        float fadeOutDuration = particle.data[2];
        float slowDownMul = particle.data[3];
        if (particle.timer < fadeInDuration)
        {
            particle.opacity += 1f / fadeInDuration;
        }
        else if (particle.timer > duration - fadeOutDuration)
        {
            particle.opacity -= 1f / fadeOutDuration;
        }

        particle.velocity *= slowDownMul;

        if (particle.timer > duration)
        {
            Kill(ref particle);
        }
    }

    public override void Draw(Particle particle)
    {
        var shader = GameShaders.Misc["NeoParacosm:FireballShader"];
        shader.Shader.Parameters["noiseStepThreshold"].SetValue(0.3f);
        shader.UseColor(particle.color);
        shader.UseImage1(ParacosmTextures.NoiseTexture);
        shader.UseOpacity(particle.opacity);
        shader.Apply();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.Draw(
            Texture.Value,
            particle.position - Main.screenPosition,
            null,
            particle.color * particle.opacity,
            0f,
            Texture.Size() * 0.5f,
            particle.scale * 0.33f,
            SpriteEffects.None,
            0
            );
        Main.spriteBatch.End();
        ParticleRenderer.BeginDefaultParticleSpriteBatch();
    }
}
