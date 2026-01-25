using Terraria.DataStructures;

namespace NeoParacosm.Content.Gores;

public class CorruptPillarGore : ModGore
{
    public override void SetStaticDefaults()
    {

    }

    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 5;
        gore.frame = (byte)Main.rand.Next(0, 5);
        gore.sticky = false;
        UpdateType = GoreID.ZombieMerman1;
    }
}
