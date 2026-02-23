using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Globals.GlobalNPCs;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    public static bool IsHard() => Main.masterMode || Main.getGoodWorld;

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

    public static void DontDropAnything(this NPC npc)
    {
        NPCID.Sets.CantTakeLunchMoney[npc.type] = true;
        NPCID.Sets.CannotDropSouls[npc.type] = true;
        NPCID.Sets.NeverDropsResourcePickups[npc.type] = true;
    }

    /// <summary>
    /// Attempts to find a safe position to teleport.
    /// </summary>
    /// <returns>Chosen position. Returns Vector2.Zero if no safe position is found</returns>
    public static Vector2 FindSafeTeleportPosition(this NPC npc, Vector2 target, float maxDistanceToTarget, float minDistanceToTarget, int maxAttemptCount = 100)
    {
        int attemptCount = 0;
        while (attemptCount < maxAttemptCount)
        {
            Vector2 chosenPos = Vector2.Zero;
            chosenPos = target + RandomVector2Circular(maxDistanceToTarget, maxDistanceToTarget, minDistanceToTarget, minDistanceToTarget);

            Point16 topLeftTile = chosenPos.ToTileCoordinates16();
            Point16 bottomRightTile = (chosenPos + new Vector2(npc.width, npc.height)).ToTileCoordinates16();
            bool badPos = false;
            for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
            {
                for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile || tile.LiquidAmount >= 200)
                    {
                        badPos = true;
                        break;
                    }

                }
                if (badPos)
                {
                    break;
                }
            }

            if (!badPos)
            {
                return chosenPos;
            }

            attemptCount++;
        }

        return Vector2.Zero;
    }

    public static void DOTDebuff(this NPC npc, float damagePerSecond, ref int damage)
    {
        if (npc.lifeRegen > 0) npc.lifeRegen = 0;
        npc.lifeRegen -= (int)(damagePerSecond * 2);
        if (damage < damagePerSecond)
        {
            damage = (int)damagePerSecond;
        }
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

    public static void DrawAfterimages(this NPC npc, Color lightColor, float opacityMultiplier = 1f)
    {
        Texture2D texture = TextureAssets.Npc[npc.type].Value;
        Rectangle sourceRect = npc.frame;
        Vector2 drawOrigin = sourceRect.Size() * 0.5f;
        for (int k = npc.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = (npc.oldPos[k] - Main.screenPosition) + drawOrigin;
            Color color = (npc.GetAlpha(lightColor) * ((npc.oldPos.Length - k) / (float)npc.oldPos.Length)) * opacityMultiplier;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0);
        }
    }
}
