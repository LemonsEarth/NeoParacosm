using Terraria.DataStructures;

namespace NeoParacosm.Content.Gores;

public class DeathbirdHeadGore : ModGore
{
    public override void SetStaticDefaults()
    {

    }

    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 1;
        gore.sticky = true;
    }
}
