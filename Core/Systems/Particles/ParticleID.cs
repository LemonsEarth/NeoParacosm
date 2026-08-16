using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Particles;
using System.Collections.Generic;

namespace NeoParacosm.Core.Systems.Particles;

/// <summary>
/// Contains all particle ids which can be used when calling ParticleSystem.SpawnParticle().
/// Particle IDs have to be assigned manually in ParticleSystem.InitializeTypesByID().
/// </summary>
public abstract class ParticleID
{
    public static void RegisterParticles()
    {
        TestParticle = ParticleSystem.RegisterParticle(new TestParticle());
        DeadForestPassiveParticle = ParticleSystem.RegisterParticle(new DeadForestPassiveParticle());
        Streak = ParticleSystem.RegisterParticle(new StreakParticle());
    }

    public static int Count => ParticleSystem.TypesByID.Count;
    public static int TestParticle;
    public static int DeadForestPassiveParticle;
    public static int Streak;
}
