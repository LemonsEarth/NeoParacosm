namespace NeoParacosm.Core.Systems.Misc;

public class KeybindSystem : ModSystem
{
    public static ModKeybind CrimsonSacrifice;

    public override void Load()
    {
        CrimsonSacrifice = KeybindLoader.RegisterKeybind(Mod, "CrimsonSacrifice", "U");
    }

    public override void Unload()
    {
        CrimsonSacrifice = null;
    }
}
