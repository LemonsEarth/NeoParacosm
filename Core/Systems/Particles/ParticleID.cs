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
        Gas = ParticleSystem.RegisterParticle(new GasParticle());
        Circle = ParticleSystem.RegisterParticle(new CircleParticle());
    }

    public static int Count => ParticleSystem.TypesByID.Count;
    public static int TestParticle;
    public static int DeadForestPassiveParticle;
    public static int Streak;

    /// <summary>
    /// data[0] - Rising speed (how fast the dust moves upward per tick)
    /// </summary>
    public static int Gas;

    /// <summary>
    /// data[0] - Minimum scale multiplier (0 => 0.5f)
    /// data[1] - Maximum scale multiplier (0 => 1f)
    /// </summary>
    public static int Circle;
}
