namespace NeoParacosm.Core.Globals.GlobalTiles;

public class DontFuckingDropBonesTile : GlobalTile
{
    public override bool CanDrop(int i, int j, int type)
    {
        if (type != TileID.BoneBlock && type != TileID.LargePiles && type != TileID.SmallPiles)
        {
            return true;
        }

        return NPC.downedBoss3;
    }
}
