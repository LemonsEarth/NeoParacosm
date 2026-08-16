using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace NeoParacosm.Core.Systems.Particles;

public struct Particle
{
    /// <summary>
    /// The ParticleID of the particle.
    /// </summary>
    public int type = -1;
    public bool active;
    /// <summary>
    /// If true, the particle will be killed during the next update.
    /// </summary>
    public bool shouldDie;
    /// <summary>
    /// Timer that increases by 1 every update.
    /// </summary>
    public int timer;

    /// <summary>
    /// World position of the particle.
    /// </summary>
    public Vector2 position;
    /// <summary>
    /// Velocity that is added to the particle's position every update;
    /// </summary>
    public Vector2 velocity;
    public float rotation;
    public float scale;

    public Color color;
    public float opacity;
    public Rectangle? frame;

    public Particle()
    {
        type = -1;
        active = false;
        shouldDie = false;
        timer = 0;
        position = Vector2.Zero;
        velocity = Vector2.Zero;
        rotation = 0f;
        scale = 1f;
        color = Color.White;
        opacity = 1f;
    }
}
