using Terraria.DataStructures;

namespace NeoParacosm.Content.Gores;

public class CorruptPillarGoreLarge : ModGore
{
    public override void SetStaticDefaults()
    {

    }

    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 4;
        gore.sticky = false;
        UpdateType = GoreID.ZombieMerman1;
    }
}
