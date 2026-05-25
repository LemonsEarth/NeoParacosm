using static Terraria.ID.TileID;

namespace NeoParacosm.Core.Globals.GlobalTiles;

public class DontFuckingDropShitTile : GlobalTile
{
    public override bool CanDrop(int i, int j, int type)
    {
        switch (type)
        {
            case BoneBlock or LargePiles or SmallPiles: // no bones pre skele
                return NPC.downedBoss3;
            case LogicSensor:
                return NPC.downedMechBoss2;
        }

        return true;
    }
}
