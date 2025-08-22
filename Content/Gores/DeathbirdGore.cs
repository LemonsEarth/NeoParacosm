using Terraria.DataStructures;

namespace NeoParacosm.Content.Gores;

public class DeathbirdGore : ModGore
{
    public override void SetStaticDefaults()
    {

    }

    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 2;
        gore.frame = (byte)Main.rand.Next(0, 2);
        gore.sticky = true;
    }
}
