using Terraria.DataStructures;

namespace NeoParacosm.Content.Gores;

public class CorruptWalkerGore : ModGore
{
    public override void SetStaticDefaults()
    {

    }

    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 3;
        gore.frame = (byte)Main.rand.Next(0, 3);
        gore.sticky = false;
        UpdateType = GoreID.ZombieMerman1;
    }
}
