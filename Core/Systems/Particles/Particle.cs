using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace NeoParacosm.Core.Systems.Particles;

public struct Particle
{
    public int type = -1;
    public bool active;

    public Vector2 position;
    public Vector2 velocity;
    public float rotation;
    public float scale;
    public int timeLeft;

    public Color color;
    public Rectangle? frame;

    public Particle()
    {
        type = -1;
        active = false;
        position = Vector2.Zero;
        velocity = Vector2.Zero;
        rotation = 0f;
        scale = 1f;
        timeLeft = 0;
        color = Color.White;
    }

    public Particle(int _type, Vector2 _position, Vector2 _velocity, Color _color = default, float _scale = 1f)
    {
        type = _type;
        position = _position;
        velocity = _velocity;
        scale = _scale;
        timeLeft = 0;
        color = _color;
    }
}
