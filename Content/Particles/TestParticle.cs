using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Particles;
using System.Collections.Generic;

namespace NeoParacosm.Content.Particles;

public class TestParticle : ParticleType
{
    public override void Update(ref Particle particle)
    {
        //particle.velocity.Y = -0.1f;
    }
}
