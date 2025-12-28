using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Effect;
using Terraria;
using Terraria.GameContent;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    public static Player GetOwner(this Projectile projectile)
    {
        return Main.player[projectile.owner];
    }

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

    public static void DrawAfterimages(this Projectile Projectile, Color lightColor, float opacityMultiplier = 1f)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle sourceRect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        Vector2 drawOrigin = sourceRect.Size() * 0.5f;
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = (Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length)) * opacityMultiplier;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
    }

    /// <summary>
    /// Gets closest chaseable NPC to a position under minDistance, returns null if none found
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="minDistance"></param>
    /// <returns></returns>
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

    public static Player GetClosestPlayer(Vector2 pos, float minDistance = 0)
    {
        Player closestPlayer = null;
        if (minDistance == 0) minDistance = 99999;
        foreach (var player in Main.ActivePlayers)
        {
            if (player.Alive() && (player.Distance(pos) < minDistance))
            {
                if (closestPlayer == null)
                {
                    closestPlayer = player;
                }
                float distanceToNPC = pos.Distance(player.Center);
                if (distanceToNPC < pos.Distance(closestPlayer.Center))
                {
                    closestPlayer = player;
                }
            }
        }
        return closestPlayer;
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
