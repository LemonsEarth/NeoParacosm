using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace NeoParacosm;

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public class NeoParacosm : Mod
{
    public override void Load()
    {
        Asset<Effect> filterShader = Assets.Request<Effect>("Common/Assets/Shaders/ScreenTintShader");
        Filters.Scene["NeoParacosm:ScreenTintShader"] = new Filter(new ScreenShaderData(filterShader, "ScreenTint"), EffectPriority.Medium);

        Asset<Effect> shieldPulseShader = Assets.Request<Effect>("Common/Assets/Shaders/ShieldPulseShader");
        GameShaders.Misc["NeoParacosm:ShieldPulseShader"] = new MiscShaderData(shieldPulseShader, "ShieldPulseShader");

        Asset<Effect> gasShader = Assets.Request<Effect>("Common/Assets/Shaders/Projectiles/GasShader");
        GameShaders.Misc["NeoParacosm:GasShader"] = new MiscShaderData(gasShader, "GasShader");

        Asset<Effect> ascendedWeaponGlowShader = Assets.Request<Effect>("Common/Assets/Shaders/Items/AscendedWeaponGlow");
        GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"] = new MiscShaderData(ascendedWeaponGlowShader, "AscendedWeaponGlow");
    }
}
