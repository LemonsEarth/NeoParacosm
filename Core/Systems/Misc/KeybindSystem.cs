namespace NeoParacosm.Core.Systems.Misc;

public class KeybindSystem : ModSystem
{
    public static ModKeybind CrimsonSacrifice;
    public static ModKeybind CycleSpellsForward;
    public static ModKeybind CycleSpellsBackward;

    public override void Load()
    {
        CrimsonSacrifice = KeybindLoader.RegisterKeybind(Mod, "CrimsonSacrifice", "U");
        CycleSpellsForward = KeybindLoader.RegisterKeybind(Mod, "CycleSpellsForward", "H");
        CycleSpellsBackward = KeybindLoader.RegisterKeybind(Mod, "CycleSpellsBackward", "G");
    }

    public override void Unload()
    {
        CrimsonSacrifice = null;
        CycleSpellsForward = null;
        CycleSpellsBackward = null;
    }
}
