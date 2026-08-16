using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Particles;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace NeoParacosm.Content.Particles;

public class DeadForestPassiveParticle : ParticleType
{
    public override void OnSpawn(ref Particle particle)
    {
        particle.frame = Texture.Frame(1, 5, 0, Main.rand.Next(0, 5));
        particle.rotation = Main.rand.NextFloat(0, 6.28f);
    }

    public override void Update(ref Particle particle)
    {
        particle.rotation += MathHelper.ToRadians(1);

        particle.color = Lighting.GetColor((int)(particle.position.X / 16), (int)(particle.position.Y / 16));

        if (particle.timer < 60)
        {
            particle.opacity = (particle.timer + 1) / 60f * 0.5f;
        }
        else if (particle.timer > 240)
        {
            particle.opacity = (1 - ((particle.timer - 240) / 60f)) * 0.5f;
        }

        if (particle.timer > 300)
        {
            Kill(ref particle);
        }
    }
}
