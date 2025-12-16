using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace NeoParacosm.Content.NPCs.Bosses.Dreadlord;

public class DreadlordBodyPart
{
    public DreadlordBodyPart()
    {

    }
    public DreadlordBodyPart(Asset<Texture2D> texture, Vector2 defaultPosition, Vector2 position, Vector2 miscPosition1, Vector2 miscPosition2, float rotation, Vector2 origin)
    {
        Texture = texture;
        DefaultPosition = defaultPosition;
        Position = position;
        MiscPosition1 = miscPosition1;
        MiscPosition2 = miscPosition2;
        Rotation = rotation;
        Origin = origin;
    }

    public Asset<Texture2D> Texture { get; set; }
    private Vector2 _defaultPosition;
    public Vector2 DefaultPosition
    {
        get
        {
            return _defaultPosition;
        }
        set
        {
            _defaultPosition = value;
            if (Position == Vector2.Zero)
            {
                Position = _defaultPosition;
            }
        }
    }
    public Vector2 Position { get; set; }
    public Vector2 MiscPosition1 { get; set; }
    public Vector2 MiscPosition2 { get; set; }
    public int Frames { get; set; } = 1;
    public int CurrentFrame { get; set; } = 0;
    public Rectangle Frame => Texture.Frame(1, Frames, 0, CurrentFrame);
    public float Rotation { get; set; } = 0f;
    public Vector2 Origin { get; set; }
    public float Scale { get; set; } = 1f;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

    public float Width => Frame.Width;
    public float Height => Frame.Height;

    public void Draw()
    {
        if (Texture == null)
        {
            return;
        }
        Main.EntitySpriteDraw(Texture.Value, Position - Main.screenPosition, Frame, Color.White, Rotation, Origin, Scale, SpriteEffects);
    }
}
