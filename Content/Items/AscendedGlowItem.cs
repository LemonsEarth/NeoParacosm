using Microsoft.Xna.Framework.Graphics;

namespace NeoParacosm.Content.Items;

public abstract class AscendedGlowItem : ModItem
{
    public virtual int OriginalItemID { get; }
    public virtual Color Color { get; }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, OriginalItemID, position, scale, frame, spriteBatch, Color);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, OriginalItemID, rotation, scale, spriteBatch, Color);
        return false;
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {

    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {

    }
}