using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    /// <param name="projectile"></param>
    /// <returns>Projectile.GetOwner()</returns>
    public static Player GetOwner(this Projectile projectile)
    {
        return Main.player[projectile.owner];
    }

    public static Projectile QuickProj(Entity entity, Vector2 position, Vector2 velocity, int type, float damage = -1, float knockback = 1, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
    {
        IEntitySource source = null;
        if (entity is Projectile proj)
        {
            if (damage == -1) damage = proj.damage;
            if (owner == -1) owner = proj.owner;
            source = proj.GetSource_FromThis();
        }
        else if (entity is NPC npc)
        {
            if (damage == -1) damage = npc.damage;

            if (!Main.expertMode)
            {
                damage *= 0.5f;
            }
            else if (Main.expertMode)
            {
                damage *= 0.25f;
            }
            else if (Main.masterMode)
            {
                damage *= 1 / 6f;
            }

            source = npc.GetSource_FromThis();
        }

        return Projectile.NewProjectileDirect(source, position, velocity, type, (int)damage, knockback, owner, ai0, ai1, ai2);
    }

    public static Texture2D GetTexture(this Projectile projectile)
    {
        return TextureAssets.Projectile[projectile.type].Value;
    }

    public static void DrawProjectile(this Projectile projectile, Color color)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        Rectangle sourceRect = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
        Vector2 drawPos = projectile.Center - Main.screenPosition;
        Vector2 drawOrigin = new Vector2(sourceRect.Width, sourceRect.Height) * 0.5f;
        Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0);
    }

    public static void DrawAfterimages(this Projectile Projectile, Color lightColor, float opacityMultiplier = 1f)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle sourceRect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        Vector2 drawOrigin = new Vector2(sourceRect.Width, sourceRect.Height) * 0.5f;
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
            Color color = (Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length)) * opacityMultiplier;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
    }

    public static bool CountsAsTrueMelee(this Projectile proj)
    {
        if (proj.TryGetOwner(out Player owner))
        {
            if (proj.friendly && proj.CountsAsClass(DamageClass.Melee) && owner.heldProj == proj.whoAmI)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets closest chaseable NPC to a position under minDistance, returns null if none found
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    public static NPC GetClosestNPC(Vector2 pos, float maxDistance = 0, params int[] excludeWhoAmIs)
    {
        NPC closestEnemy = null;
        if (maxDistance == 0) maxDistance = 99999;
        foreach (var npc in Main.ActiveNPCs)
        {
            if (excludeWhoAmIs.Contains(npc.whoAmI))
            {
                continue;
            }
            if (npc.CanBeChasedBy() && (npc.Distance(pos) < maxDistance))
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

    public static Player GetClosestPlayer(Vector2 pos, float maxDistance = 0)
    {
        Player closestPlayer = null;
        if (maxDistance == 0) maxDistance = 99999;
        foreach (var player in Main.ActivePlayers)
        {
            if (player.Alive() && (player.Distance(pos) < maxDistance))
            {
                if (closestPlayer == null)
                {
                    closestPlayer = player;
                }
                float distanceToCurrentPlayer = pos.Distance(player.Center);
                if (distanceToCurrentPlayer < pos.Distance(closestPlayer.Center))
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
