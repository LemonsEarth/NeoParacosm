using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

namespace NeoParacosm.Content.Dusts
{
    public class StreakDust : ModDust
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
            dust.noGravity = true; // Makes the dust have no gravity.
            dust.noLight = true; // Makes the dust emit no light.
            dust.frame = new Rectangle(0, Main.rand.Next(0, 4) * 50, 50, 50);
            dust.scale *= 0.5f;
            dust.velocity *= 4;
        }

        public override bool Update(Dust dust)
        { // Calls every frame the dust is active
            dust.position += dust.velocity;
            dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;
            dust.scale *= 0.97f;
            dust.velocity *= 0.94f;
            dust.color = Color.Lerp(dust.color, Color.White, 1 / 30f);
            if (!dust.noGravity)
            {
                dust.velocity.Y += 0.1f;
            }
            float light = 0.35f * dust.scale;

            Lighting.AddLight(dust.position, light, light, light);

            if (dust.scale < 0.2f)
            {
                dust.active = false;
            }

            return false; // Return false to prevent vanilla behavior.
        }

        public override bool PreDraw(Dust dust)
        {
            Vector2 scale = new Vector2(dust.scale, dust.scale * 2);
            Main.spriteBatch.Draw(texture.Value, dust.position - Main.screenPosition, dust.frame, dust.color, dust.rotation, new Vector2(25, 25), scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
