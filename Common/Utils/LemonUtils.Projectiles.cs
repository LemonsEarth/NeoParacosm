namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    public static Projectile QuickProj(Projectile proj, Vector2 position, Vector2 velocity, int type, float damage = -1, float knockback = 1, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
    {
        if (damage == -1) damage = proj.damage;
        if (owner == -1) owner = proj.owner;
        return Projectile.NewProjectileDirect(proj.GetSource_FromThis(), position, velocity, type, (int)damage, knockback, owner, ai0, ai1, ai2);
    }

    public static Projectile QuickProj(NPC npc, Vector2 position, Vector2 velocity, int type, float damage = -1, float knockback = 1, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
    {
        if (damage == -1) damage = npc.damage;
        return Projectile.NewProjectileDirect(npc.GetSource_FromThis(), position, velocity, type, (int)damage, knockback, owner, ai0, ai1, ai2);
    }

    public static NPC GetClosestNPC(Vector2 pos, float minDistance = 0)
    {
        NPC closestEnemy = null;
        if (minDistance == 0) minDistance = 99999;
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.CanBeChasedBy() && (npc.Distance(pos) < minDistance))
            {
                if (closestEnemy == null)
                {
                    closestEnemy = npc;
                }
                float distanceToNPC = pos.Distance(npc.Center);
                if (distanceToNPC < pos.Distance(closestEnemy.Center))
                {
                    closestEnemy = npc;
                }
            }
        }
        return closestEnemy;
    }

    public static void StandardAnimation(this Projectile proj, int frameDuration, int maxFrames)
    {
        proj.frameCounter++;
        if (proj.frameCounter == frameDuration)
        {
            proj.frame++;
            proj.frameCounter = 0;
            if (proj.frame >= maxFrames)
            {
                proj.frame = 0;
            }
        }
    }

    public static Vector2 RandomPos(this Projectile proj, float fluffX = 0, float fluffY = 0)
    {
        Vector2 pos = proj.position + new Vector2(Main.rand.NextFloat(-fluffX, proj.width + fluffX), Main.rand.NextFloat(-fluffY, proj.height + fluffY));
        return pos;
    }

}
