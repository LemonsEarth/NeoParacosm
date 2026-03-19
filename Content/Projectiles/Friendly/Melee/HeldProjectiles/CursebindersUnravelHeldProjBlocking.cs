using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Players;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee.HeldProjectiles;

public class CursebindersUnravelHeldProjBlocking : PrimProjectile
{
    int AITimer = 0;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 180;
        Projectile.extraUpdates = 3;
    }

    float rotValue = -60;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.Alive())
        {
            Projectile.Kill();
        }

        if (AITimer == 0)
        {
            player.GetModPlayer<DeflectPlayer>().StartBlocking(15);
            rotValue = -30;
        }

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(15);

        Projectile.velocity = Vector2.Zero;
        float goalRotation = 180;
        float lerpSpeed = 1 / 12f;

        rotValue = MathHelper.Lerp(rotValue, goalRotation, lerpSpeed * player.GetAttackSpeed(DamageClass.Melee));
        //if (rotValue > goalRotation - 10) Projectile.Kill();
        SetPositionRotationDirection(player, MathHelper.ToRadians(rotValue));
        AITimer++;
    }

    public void Restart()
    {
        rotValue = -60;
        Projectile.timeLeft = 60;
    }

    public override void OnKill(int timeLeft)
    {

    }

    const float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 pos = player.Center + new Vector2(-player.direction * (Projectile.width / 2), -Projectile.height / 2).RotatedBy(movedRotation * player.direction) * Projectile.scale;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation * player.direction + player.direction * ThreePiOverFour);
        Projectile.Center = pos;
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Projectile.GetOwner();
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        float topRotOffset = player.direction == 1 ? -MathHelper.PiOver4 : -3 * MathHelper.PiOver4;
        float botRotOffset = player.direction == 1 ? 3 * MathHelper.PiOver4 : MathHelper.PiOver4;

        //Main.spriteBatch.End(); // Restarting spritebatch around Primitive Drawing to fix some layering issues
        //PrimHelper.DrawHeldProjectilePrimTrailRectangular(Projectile, Color.DarkRed, Color.Transparent, BasicEffect, topRotOffset, botRotOffset, (int)(Projectile.height * 0.7f), (int)(Projectile.height * 0.7f));
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        if (AITimer > 10)
        {
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 AIdrawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
                Color color = (Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length)) * 0.5f;
                Main.EntitySpriteDraw(texture, AIdrawPos, null, color, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
            }
        }
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
