using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace NeoParacosm;

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public class NeoParacosm : Mod
{
    public NeoParacosm()
    {
        MusicSkipsVolumeRemap = true;
    }

    public override void Load()
    {
        LoadFilterShader("ScreenTintShader", "Common/Assets/Shaders/ScreenTintShader", EffectPriority.Medium);
        LoadFilterShader("DesaturateShader", "Common/Assets/Shaders/DesaturateShader", EffectPriority.Medium);

        LoadMiscShader("ShieldPulseShader", "Common/Assets/Shaders/Projectiles/ShieldPulseShader");
        LoadMiscShader("GasShader", "Common/Assets/Shaders/Projectiles/GasShader");
        LoadMiscShader("LaserShader", "Common/Assets/Shaders/Projectiles/LaserShader");
        LoadMiscShader("AscendedWeaponGlow", "Common/Assets/Shaders/Items/AscendedWeaponGlow");
        LoadMiscShader("DeathbirdWingShader", "Common/Assets/Shaders/NPCs/DeathbirdWingShader");
    }

    void LoadFilterShader(string name, string path, EffectPriority priority)
    {
        Asset<Effect> filter = Assets.Request<Effect>(path);
        Filters.Scene[$"NeoParacosm:{name}"] = new Filter(new ScreenShaderData(filter, name), priority);
    }

    void LoadMiscShader(string name, string path)
    {
        Asset<Effect> shader = Assets.Request<Effect>(path);
        GameShaders.Misc[$"NeoParacosm:{name}"] = new MiscShaderData(shader, name);
    }
}
