using Terraria.DataStructures;
using Terraria.GameContent;

namespace NeoParacosm.Content.Gores;

public class RedSmokeGore : ModGore
{
    public override void SetStaticDefaults()
    {
        ChildSafety.SafeGore[Type] = true;
    }

    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 3;
        gore.frame = (byte)Main.rand.Next(0, 3);
        gore.sticky = false;
        UpdateType = GoreID.Smoke1;
    }
}
