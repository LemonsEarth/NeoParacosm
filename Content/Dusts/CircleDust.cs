using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace NeoParacosm.Content.Dusts;

public class CircleDust : ModDust
{
    static Asset<Texture2D> texture;
    public override void SetStaticDefaults()
    {
        texture = Request<Texture2D>(Texture);
    }

    public override void OnSpawn(Dust dust)
    {
        if (dust.color == new Color(0, 0, 0, 0))
        {
            dust.color = Color.White;
        }
        dust.frame = new Rectangle(0, Main.rand.Next(0, 4) * 32, 32, 32);
    }

    public override bool Update(Dust dust)
    { // Calls every frame the dust is active
        dust.alpha += 4;
        dust.position += dust.velocity;
       // dust.rotation += MathHelper.ToRadians(5);
        //dust.scale *= 0.94f;
        dust.velocity *= 0.98f;
        dust.color = Color.Lerp(dust.color, Color.Transparent, 1 / 90f);
        if (!dust.noGravity)
        {
            dust.velocity.Y -= Main.rand.NextFloat(0.08f, 0.12f);
        }
        //float light = 0.35f * dust.scale;

        //Lighting.AddLight(dust.position, light, light, light);

        if (dust.alpha > 220)
        {
            dust.active = false;
        }

        return false; // Return false to prevent vanilla behavior.
    }

    public override bool PreDraw(Dust dust)
    {
        Vector2 scale = new Vector2(dust.scale, dust.scale);
        float opacity = 1f - (dust.alpha / 255f);
        //LemonUtils.DrawGlow(dust.position, dust.color, opacity, dust.scale);
        Main.spriteBatch.Draw(texture.Value, dust.position - Main.screenPosition, dust.frame, dust.color * opacity, dust.rotation, new Vector2(16, 16), scale, SpriteEffects.None, 0f);
        return false;
    }
}
