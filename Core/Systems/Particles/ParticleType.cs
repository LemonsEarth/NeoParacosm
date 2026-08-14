using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace NeoParacosm.Core.Systems.Particles;

public abstract class ParticleType : ILoadable
{
    public Asset<Texture2D> Texture { get; protected set; }

    public virtual void Draw(Particle particle)
    {
        Main.spriteBatch.Draw(
            Texture.Value,
            particle.position - Main.screenPosition,
            particle.frame,
            particle.color,
            particle.rotation,
            particle.frame == null ? Texture.Size() * 0.5f : particle.frame.Value.Size() * 0.5f,
            particle.scale,
            SpriteEffects.None,
            0
            );
    }

    public virtual void Update(ref Particle particle)
    {

    }

    public void Load(Mod mod)
    {
        string path = $"{GetType().Namespace}/{GetType().Name}".Replace(".", "/");
        Texture = Request<Texture2D>(path);
    }

    public void Unload()
    {
        Texture.Dispose();
    }
}
