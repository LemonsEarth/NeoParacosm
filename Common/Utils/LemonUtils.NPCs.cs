using NeoParacosm.Core.Globals.GlobalNPCs;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    public static bool IsHard() => Main.masterMode || Main.getGoodWorld;

    public static NPBuffNPC NPBuffNPC(this NPC npc)
    {
        return npc.GetGlobalNPC<NPBuffNPC>();
    }

    public static NPGlobalNPC NP(this NPC npc)
    {
        return npc.GetGlobalNPC<NPGlobalNPC>();
    }

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

    /// <summary>
    /// Used by custom NPCs such as the Researcher which don't use normal NPC interact behavior
    /// </summary>
    /// <returns>Whether the *local player* can talk to the NPC or not</returns>
    public static bool CanTalkToNPC(NPC npc, float maxTalkDistance)
    {
        return Main.LocalPlayer.Alive()
        && npc.Hitbox.Contains(Main.MouseWorld.ToPoint())
        && Main.LocalPlayer.Distance(npc.Center) < maxTalkDistance
        && Main.mouseRight && Main.mouseRightRelease;
    }
}
