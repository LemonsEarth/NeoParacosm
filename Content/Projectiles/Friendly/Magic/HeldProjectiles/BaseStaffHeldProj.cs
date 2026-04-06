using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;

public abstract class BaseStaffHeldProj : ModProjectile
{
    public int AITimer { get; set; } = 0;

    public virtual int ManaConsumeInterval => 10;

    public void HeldProjectileControl(Vector2 targetPosition, bool checkLeftClick = true)
    {
        Player player = Projectile.GetOwner();
        if (!player.Alive())
        {
            Projectile.Kill();
        }
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        if (checkLeftClick && player.channel)
        {
            Projectile.timeLeft = 2;
            SetPositionRotationDirection(player, targetPosition);
            if (AITimer % ManaConsumeInterval == 0)
            {
                if (!player.CheckMana(player.HeldItem.mana, true))
                {
                    Projectile.Kill();
                    return;
                }
            }
        }
        else if (!checkLeftClick)
        {
            Projectile.timeLeft = 2;
            SetPositionRotationDirection(player, targetPosition);
            if (AITimer % ManaConsumeInterval == 0)
            {
                if (!player.CheckMana(player.HeldItem.mana, true))
                {
                    Projectile.Kill();
                    return;
                }
            }
        }
        else
        {
            Projectile.Kill();
            return;
        }

    }

    /// <summary>
    /// Sets the arm and staff to rotate towards the cursor
    /// </summary>
    /// <param name="player"></param>
    public void SetPositionRotationDirection(Player player, Vector2 targetPosition)
    {
        Vector2 dir = player.Center.DirectionTo(targetPosition);
        if (!dir.HasNaNs())
        {
            player.ChangeDir(Math.Sign(dir.X));
            Projectile.spriteDirection = Math.Sign(dir.X);
        }
        float movedRotation = dir.ToRotation();
        float armRotValue = -MathHelper.PiOver2;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation + armRotValue);
        Projectile.Center = player.Center + dir * (Projectile.Size * 0.5f).Length();
        Projectile.rotation = movedRotation + MathHelper.PiOver4;
        Projectile.spriteDirection = 1;
    }
}
