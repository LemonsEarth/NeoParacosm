using NeoParacosm.Core.Systems.Particles.Renderers;
using System.Collections.Generic;

namespace NeoParacosm.Core.Systems.Particles;

public class ParticleSystem : ModSystem
{
    /// <summary>
    /// Maps ParticleID values to ParticleType instances.
    /// </summary>
    public static List<ParticleType> TypesByID { get; private set; } = new List<ParticleType>();

    public override void SetStaticDefaults()
    {
        InitializeTypesByID();
    }
    /// <summary>
    /// Adds the ParticleType to the list of particle types.
    /// </summary>
    /// <param name="typeInstance"></param>
    /// <returns>The new ParticleID of the added particle type.</returns>
    public static int RegisterParticle(ParticleType typeInstance)
    {
        int particleID = TypesByID.Count;
        TypesByID.Add(typeInstance);
        return particleID;
    }

    /// <summary>
    /// Registers all particle types and calls load on them (for example, to load textures).
    /// </summary>
    public static void InitializeTypesByID()
    {
        ParticleID.RegisterParticles();

        for (int i = 0; i < ParticleID.Count; i++)
        {
            TypesByID[i].Load(NeoParacosm.Instance);
        }
    }

    /// <summary>
    /// Spawns a new particle into the world. Returns a reference to the newly spawned particle.
    /// The data# params are custom data in the particle.data[] array. Their purpose depends on the particle type.
    /// </summary>
    /// <param name="type">The ParticleID of the particle.</param>
    /// <param name="position">World position of the particle.</param>
    /// <param name="velocity"></param>
    /// <param name="color">Color to draw the particle in. Default is Color.White.<br></br>
    /// Change the opacity param if you only want to change visibility.</param>
    /// <param name="opacity">Opacity of the particle.</param>
    /// <param name="scale"></param>
    /// <returns>A reference to the newly spawned particle.</returns>
    public static ref Particle SpawnParticle<T>(
        int type,
        Vector2 position,
        Vector2 velocity,
        Color color = default,
        float opacity = 1f,
        float scale = 1f,
        float data0 = 0f,
        float data1 = 0f,
        float data2 = 0f,
        float data3 = 0f)
        where T : ParticleRenderer
    {
        ref var particle = ref GetInstance<T>().SpawnParticle(type, position, velocity, color, opacity, scale, data0, data1, data2, data3);
        return ref particle;
    }

    /// <summary>
    /// Spawns a new particle into the world. Returns a reference to the newly spawned particle.
    /// The data# params are custom data in the particle.data[] array. Their purpose depends on the particle type.
    /// </summary>
    /// <param name="type">The ParticleID of the particle.</param>
    /// <param name="position">World position of the particle.</param>
    /// <param name="velocity"></param>
    /// <param name="color">Color to draw the particle in. Default is Color.White.<br></br>
    /// Change the opacity param if you only want to change visibility.</param>
    /// <param name="opacity">Opacity of the particle.</param>
    /// <param name="scale"></param>
    /// <returns>A reference to the newly spawned particle.</returns>
    public static ref Particle SpawnParticle(
        int type,
        Vector2 position,
        Vector2 velocity,
        Color color = default,
        float opacity = 1f,
        float scale = 1f,
        float data0 = 0f,
        float data1 = 0f,
        float data2 = 0f,
        float data3 = 0f)
    {
        ref var particle = ref GetInstance<AfterDustParticleRenderer>().SpawnParticle(type, position, velocity, color, opacity, scale, data0, data1, data2, data3);
        return ref particle;
    }
}
