using NeoParacosm.Content.Particles;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace NeoParacosm.Core.Systems.Particles;

public class ParticleSystem : ModSystem
{
    public const int MAX_PARTICLES = 10000;
    public static Particle[] Particles { get; private set; } = new Particle[MAX_PARTICLES];
    public static int ActiveParticleCount { get; private set; } = 0;
    public static ParticleType[] TypesByID { get; private set; } = new ParticleType[ParticleID.Count];

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

    public void InitializeTypesByID()
    {
        TypesByID[ParticleID.TestParticle] = new TestParticle();

        for (int i = 0; i < ParticleID.Count; i++)
        {
            TypesByID[i].Load(Mod);
        }
    }

    public void InitializeParticles()
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
            particle.timeLeft--;
            if (particle.timeLeft <= 0)
            {
                KillParticle(i);
                i--;
            }
        }
    }

    public static void SpawnParticle(int type, Vector2 position, Vector2 velocity, Color color = default, float scale = 1f)
    {
        if (ActiveParticleCount >= MAX_PARTICLES)
        {
            return;
        }

        if (color == default)
        {
            color = Color.White;
        }

        Particles[ActiveParticleCount].active = true;
        Particles[ActiveParticleCount].type = type;
        Particles[ActiveParticleCount].position = position;
        Particles[ActiveParticleCount].velocity = velocity;
        Particles[ActiveParticleCount].color = color;
        Particles[ActiveParticleCount].scale = scale;
        Particles[ActiveParticleCount].timeLeft = 60;
        ActiveParticleCount++;
    }

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
