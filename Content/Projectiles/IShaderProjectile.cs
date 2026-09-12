using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles;

public interface IShaderProjectile
{
    public MiscShaderData ShaderData { get; }

    public void DrawProjectile();
}
