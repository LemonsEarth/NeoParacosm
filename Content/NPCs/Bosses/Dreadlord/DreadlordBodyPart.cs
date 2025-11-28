using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace NeoParacosm.Content.NPCs.Bosses.Dreadlord;

public class DreadlordBodyPart
{
    public DreadlordBodyPart()
    {

    }
    public DreadlordBodyPart(Asset<Texture2D> texture, Vector2 position, Vector2 miscPosition, float rotation, Vector2 origin)
    {
        Texture = texture;
        Position = position;
        MiscPosition = miscPosition;
        Rotation = rotation;
        Origin = origin;
    }

    public Asset<Texture2D> Texture { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 MiscPosition { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

    public void Draw()
    {
        if (Texture == null)
        {
            return;
        }
        Main.EntitySpriteDraw(Texture.Value, Position - Main.screenPosition, null, Color.White, Rotation, Origin, 1f, SpriteEffects);
    }
}
