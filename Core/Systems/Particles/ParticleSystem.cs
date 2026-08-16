using NeoParacosm.Content.Particles;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace NeoParacosm.Core.Systems.Particles;

public class ParticleSystem : ModSystem
{
    public const int MAX_PARTICLES = 50000;
    public static Particle[] Particles { get; private set; } = new Particle[MAX_PARTICLES];
    public static int ActiveParticleCount { get; private set; } = 0;
    public static List<ParticleType> TypesByID { get; private set; } = new List<ParticleType>();

    public override void Load()
    {
        On_Main.DrawDust += On_Main_DrawDust;
    }

    private void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        DrawParticles();
    }

    public override void SetStaticDefaults()
    {
        InitializeTypesByID();
        InitializeParticles();
    }

    public override void ClearWorld()
    {
        ActiveParticleCount = 0;
        InitializeParticles();
    }

    /// <summary>
    /// Adds the ParticleType instance to the list of particle types.
    /// </summary>
    /// <param name="typeInstance"></param>
    /// <returns>its index within the list.</returns>
    public static int RegisterParticle(ParticleType typeInstance)
    {
        int particleID = TypesByID.Count;
        TypesByID.Add(typeInstance);
        return particleID;
    }

    /// <summary>
    /// Adds particle types to TypesByID and sets the appropriate ParticleID value.
    /// When creating new particles, make sure to add them here as well as in ParticleID.
    /// </summary>
    public void InitializeTypesByID()
    {
        ParticleID.RegisterParticles();

        for (int i = 0; i < ParticleID.Count; i++)
        {
            TypesByID[i].Load(Mod);
        }
    }

    public static void InitializeParticles()
    {
        for (int i = 0; i < MAX_PARTICLES; i++)
        {
            Particles[i] = new Particle();
        }
    }

    public static void DrawParticles()
    {
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        for (int i = 0; i < ActiveParticleCount; i++)
        {
            TypesByID[Particles[i].type].Draw(Particles[i]);
        }
        Main.spriteBatch.End();
    }

    public override void PostUpdateDusts()
    {
        UpdateParticles();
    }

    public static void UpdateParticles()
    {
        for (int i = 0; i < ActiveParticleCount; i++)
        {
            ref Particle particle = ref Particles[i];
            TypesByID[particle.type].Update(ref particle);
            particle.position += particle.velocity;
            particle.timer++;
            if (particle.shouldDie)
            {
                KillParticle(i);
                i--;
            }
        }
    }

    static int ReplacementIndex = 0;
    public static ref Particle SpawnParticle(int type, Vector2 position, Vector2 velocity, Color color = default, float opacity = 1f, float scale = 1f)
    {
        if (color == default)
        {
            color = Color.White;
        }

        // Replacing "old" particles
        if (ActiveParticleCount >= MAX_PARTICLES)
        {
            ref Particle particle1 = ref Particles[ReplacementIndex];
            particle1.active = true;
            particle1.type = type;
            particle1.position = position;
            particle1.velocity = velocity;
            particle1.color = color;
            particle1.opacity = opacity;
            particle1.scale = scale;
            particle1.shouldDie = false;
            particle1.timer = 0;
            TypesByID[particle1.type].OnSpawn(ref particle1);
            ReplacementIndex++;
            if (ReplacementIndex >= MAX_PARTICLES)
            {
                ReplacementIndex = 0;
            }
            return ref particle1;
        }
        ReplacementIndex = 0;

        ref Particle particle = ref Particles[ActiveParticleCount];
        particle.active = true;
        particle.type = type;
        particle.position = position;
        particle.velocity = velocity;
        particle.color = color;
        particle.opacity = opacity;
        particle.scale = scale;
        particle.shouldDie = false;
        particle.timer = 0;
        TypesByID[particle.type].OnSpawn(ref particle);
        ActiveParticleCount++;
        return ref particle;
    }

    /// <summary>
    /// Kills the particle at index. 
    /// Switches the spots of that particle and the last active particle to make sure all active particles are next to each other in the array.
    /// </summary>
    /// <param name="index"></param>
    public static void KillParticle(int index)
    {
        if (ActiveParticleCount == 1)
        {
            Particles[index].active = false;
        }
        else
        {
            int lastActiveParticleIDX = ActiveParticleCount - 1;
            Particles[index] = Particles[lastActiveParticleIDX];
            Particles[lastActiveParticleIDX].active = false;
        }
        ActiveParticleCount--;
    }
}
