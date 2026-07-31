global using Microsoft.Xna.Framework;
global using System;
global using Terraria;
global using Terraria.ID;
global using Terraria.ModLoader;
global using NeoParacosm.Common.Utils;
global using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using NeoParacosm.Core.CustomSkies.Biome;

namespace NeoParacosm;

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public class NeoParacosm : Mod
{
    public static NeoParacosm Instance { get; private set; }

    public NeoParacosm()
    {
        MusicAutoloadingEnabled = false;
        MusicSkipsVolumeRemap = true;
    }

    public override void Load()
    {
        Instance = this;
        LoadFilterShader("ScreenTintShader", "Common/Assets/Shaders/ScreenTintShader", EffectPriority.Medium);
        LoadFilterShader("DesaturateShader", "Common/Assets/Shaders/DesaturateShader", EffectPriority.Medium);
        LoadFilterShader("DeathbirdArenaShader", "Common/Assets/Shaders/NPCs/DeathbirdArenaShader", EffectPriority.High);
        LoadFilterShader("NauseaShader", "Common/Assets/Shaders/NauseaShader", (EffectPriority)9999);
        LoadFilterShader("DCEffect", "Common/Assets/Shaders/Biome/DCEffect", (EffectPriority)9999);
        LoadFilterShader("DCDomainEffect", "Common/Assets/Shaders/Biome/DCDomainEffect", (EffectPriority)9999);

        SkyManager.Instance["NeoParacosm:DCSky"] = new DCSky();
        SkyManager.Instance["NeoParacosm:DCDomainSky"] = new DCDomainSky();
    }

    void LoadFilterShader(string name, string path, EffectPriority priority)
    {
        Asset<Effect> filter = Assets.Request<Effect>(path);
        Filters.Scene[$"NeoParacosm:{name}"] = new Filter(new ScreenShaderData(filter, name), priority);
    }
}
