using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;

namespace NeoParacosm.Content.Projectiles
{
    public abstract class PrimProjectile : ModProjectile
    {
        protected static BasicEffect BasicEffect;
        protected static GraphicsDevice GraphicsDevice => Main.instance.GraphicsDevice;
        public override void Load()
        {
            LoadBasicEffect();
        }

        public override void Unload()
        {
            UnloadBasicEffect();
        }

        public static void LoadBasicEffect()
        {
            if (Main.dedServ) return;
            Main.RunOnMainThread(() =>
            {
                BasicEffect = new BasicEffect(PrimHelper.GraphicsDevice)
                {
                    TextureEnabled = true,
                    VertexColorEnabled = true,
                };
            });
        }

        public static void UnloadBasicEffect()
        {
            if (Main.dedServ) return;
            Main.RunOnMainThread(() =>
            {
                BasicEffect?.Dispose();
                BasicEffect = null;
            });
        }
    }
}
