namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    public static NPC GetClosestNPC(NPC npc, float minDistance = 0)
    {
        NPC closestEnemy = null;
        foreach (var _npc in Main.ActiveNPCs)
        {
            if (_npc.CanBeChasedBy() && (minDistance > 0 && _npc.Distance(npc.Center) < minDistance))
            {
                if (closestEnemy == null)
                {
                    closestEnemy = _npc;
                }
                float distanceToNPC = npc.Center.Distance(_npc.Center);
                if (distanceToNPC < npc.Center.Distance(closestEnemy.Center))
                {
                    closestEnemy = _npc;
                }
            }
        }
        return closestEnemy;
    }

    public static Vector2 RandomPos(this NPC npc, float fluffX = 0, float fluffY = 0)
    {
        Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(-fluffX, npc.width + fluffX), Main.rand.NextFloat(-fluffY, npc.height + fluffY));
        return pos;
    }
}
